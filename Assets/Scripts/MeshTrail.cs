using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 5f;               //잔상 효과 지속 시간

    [Header("Mesh Releted")]                    //메시(3D 모델) 관련 설정
    public float meshRefreshRate = 0.05f;        //잔상이 생성되는 시간 간격
    public float meshDestoryDelay = 0.5f;       //생성된 잔상이 사라지는 데 걸리는 시간
    public Transform positionToSpawn;           //잔상이 생성될 위치

    [Header("Shader Releted")]                  //셰이더 관련 설정
    public Material mat;                        //잔상에 적용될 재질
    public string shaderVarRef;                 //셰이더에서 사용할 변수 이름 (Alpha)
    public float shaderVarRate = 0.1f;          //셰이더 효과의 변화 속도
    public float shaderVarRefreshRate = 0.05f;  //셰이더 효과가 업데이트 되는 시간 간격

    private SkinnedMeshRenderer[] skinnedRenderer;      //캐릭터의 3D 모델을 랜더링 하는 컴포넌트들
    private bool isTrailActive;                         //현재 잔상 효과가 활성화되어 있는지 확인하는 변수


    //재질의 투명도를 서서히 변경하는 코루틴
    IEnumerator AnimateMaterialFloat(Material m, float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVarRef);

        //목표값에 도달할때까지 반복
        while (valueToAnimate > valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator ActivateTrail(float timeActivated)
    {
        while (timeActivated > 0)
        {
            timeActivated -= meshRefreshRate;

            if (skinnedRenderer == null) //스킨드 메시 렌더러 컴포넌트들을 가져옴
                skinnedRenderer = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedRenderer.Length; i++)     //각 메시 렌더러에 대한 잔상 생성
            {
                GameObject gObj = new GameObject();     //새로운 오브젝트 생성
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();        //현재 캐릭터의 포즈를 메시로 변환
                skinnedRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;
                //잔상의 페이드아웃 효과 시작 
                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestoryDelay);    //일정 시간 후 잔상 제거
            }
            //다음 잔상 생성까지 대기
            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }

    public void Use()
    {
        if (!isTrailActive)       //스페이스바를 누르고 현재 잔상 효과가 비활성화 상태일 때
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));      //잔상 효과 코루틴 시작
        }
    }

}
