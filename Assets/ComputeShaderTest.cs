using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader computeShader;
    public ComputeShader trailShader;
    public ComputeShader debugShader;
    public RenderTexture renderTexture;
    ComputeBuffer agentBuffer;
    public int numAgents;
    public int moveSpeed;
    public float evaporateSpeed;
    public float diffuseSpeed;
    public int width;
    public int height;
    public float turnSpeed;
    public float degreeFOV;
    public int sensorOffsetDst;
    public int sensorSize;
    public bool debug = true;

    private Agent[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = new Agent[numAgents];
        agentBuffer = new ComputeBuffer(agents.Length, Marshal.SizeOf(typeof(Agent)));
        for (int i = 0; i < agents.Length; i++) // change to for loop
        {
            agents[i].position.x = width / 2;
            agents[i].position.y = height / 2;
            agents[i].angle = Random.Range(0, 2 * Mathf.PI);
            // if (i < 100) {
            //     agents[i].debug = 1;
            // }
        }
        // Transfer data to compute buffer after updating the agents array
        agentBuffer.SetData(agents);
    }


    private void FixedUpdate() {
        if (renderTexture == null) {
            renderTexture = new RenderTexture(width, height, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        // if (debug) {
        //     debugShader.SetTexture(0, "pixels", renderTexture);
        //     debugShader.SetInt("width", renderTexture.width);
        //     debugShader.SetInt("height", renderTexture.height);
        //     debugShader.SetInt("numAgents", numAgents);
        //     debugShader.SetInt("sensorOffsetDst", sensorOffsetDst);
        //     debugShader.SetInt("sensorSize", sensorSize);
        //     agentBuffer.SetData(agents);
        //     debugShader.SetBuffer(0, "agents", agentBuffer);
        //     debugShader.Dispatch(0, agents.Length / 8, 1, 1);
        // }
        computeShader.SetTexture(0, "TrailMap", renderTexture);
        computeShader.SetInt("width", renderTexture.width);
        computeShader.SetInt("height", renderTexture.height);
        computeShader.SetInt("numAgents", numAgents);
        computeShader.SetInt("moveSpeed", moveSpeed);
        computeShader.SetInt("sensorOffsetDst", sensorOffsetDst);
        computeShader.SetInt("sensorSize", sensorSize);
        computeShader.SetFloat("deltaTime", Time.fixedDeltaTime);
        computeShader.SetFloat("turnSpeed", turnSpeed);
        computeShader.SetFloat("degreeFOV", degreeFOV);
        agentBuffer.SetData(agents);
        computeShader.SetBuffer(0, "agents", agentBuffer);
        computeShader.Dispatch(0, agents.Length / 8, 1, 1);
        agentBuffer.GetData(agents);

        trailShader.SetTexture(0, "TrailMap", renderTexture);
        trailShader.SetInt("width", renderTexture.width);
        trailShader.SetInt("height", renderTexture.height);
        trailShader.SetFloat("evaporateSpeed", evaporateSpeed);
        trailShader.SetFloat("diffuseSpeed", diffuseSpeed);
        trailShader.SetFloat("deltaTime", Time.fixedDeltaTime);
        trailShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(renderTexture, dest);
    }

    public struct Agent
    {
        public Vector2 position;
		public float angle;
        public int debug;
    }

    private void OnDisable() {
        agentBuffer.Release();
    }

}
