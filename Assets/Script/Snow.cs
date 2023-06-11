using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class Snow : MonoBehaviour {
    public float depth;

    public int textureSize = 1024;
    public float speed = 10;

    public Shader paintShader;
    public Transform moveObj;
    public float maxDistance = 1;
    public float interalTime = 5;

    private RenderTexture lastTexture;
    private RenderTexture curTexture;
    private Material paintMaterial;
    private Renderer myRenderer;

    private int footPrintTextureID = Shader.PropertyToID("_FootprintTex");
    private float curDistance = 0;

    int debugUVID = Shader.PropertyToID("_DebugUV");
    int positionID = Shader.PropertyToID("_Position");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int colorID = Shader.PropertyToID("_Color");
    int textureID = Shader.PropertyToID("_MainTex");


    private void Start() {
        lastTexture = RenderTexture.GetTemporary(textureSize, textureSize, 0);
        lastTexture.filterMode = FilterMode.Bilinear;
        curTexture = RenderTexture.GetTemporary(textureSize, textureSize, 0);
        curTexture.filterMode = FilterMode.Bilinear;
        ClearRT(lastTexture);
        myRenderer = GetComponent<Renderer>();
        myRenderer.material.SetTexture(footPrintTextureID, lastTexture);

        paintMaterial = new Material(paintShader);
    }

    private void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0) {
            moveObj.position += new Vector3(x, 0, y) * speed * Time.deltaTime;
            curDistance += (MathF.Abs(x) + MathF.Abs(y)) * speed * Time.deltaTime;
            if (curDistance > maxDistance) {
                curDistance = 0;
                Paint(moveObj.position);
                StartCoroutine(ClearFootprint(moveObj.position));
            }
        }
    }

    private void ClearRT(RenderTexture renderTexture) {
        CommandBuffer command = new CommandBuffer();
        command.SetRenderTarget(renderTexture);
        command.ClearRenderTarget(true, true, Color.clear);
        Graphics.ExecuteCommandBuffer(command);
        command.Release();
    }
    private void Paint(Vector3 pos, float radius = 1f,
                float hardness = .5f, float strength = .5f, Color? color = null) {

        CommandBuffer commandBuffer = new CommandBuffer();
        paintMaterial.SetInt(debugUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, lastTexture);

        commandBuffer.SetRenderTarget(curTexture);
        commandBuffer.DrawRenderer(myRenderer, paintMaterial);

        commandBuffer.SetRenderTarget(lastTexture);
        commandBuffer.Blit(curTexture, lastTexture);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Release();
    }

    private IEnumerator ClearFootprint(Vector3 pos, float radius = 1f,
                float hardness = .5f, float strength = .5f, Color? color = null) {
        yield return new WaitForSeconds(interalTime);
        CommandBuffer command = new CommandBuffer();
        CommandBuffer commandBuffer = new CommandBuffer();
        paintMaterial.SetInt(debugUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetColor(colorID, color ?? Color.black);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, lastTexture);

        commandBuffer.SetRenderTarget(curTexture);
        commandBuffer.DrawRenderer(myRenderer, paintMaterial);

        commandBuffer.SetRenderTarget(lastTexture);
        commandBuffer.Blit(curTexture, lastTexture);

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Release();

    }
}
