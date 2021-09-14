using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGeneration : MonoBehaviour
{
    float m_CameraHeight;       // height of camera
    float m_CameraWidth;        // width of camera

    float k_PlatformMinX;        // Min (Left) X coordinate to spawn a platform
    float k_PlatformMaxX;        // Max (Right) X coordinate to spawn a platform

    const float k_PlatformMaxGapX = 2.7f;     // Maximum horizontal distance between 2 platforms
    const float k_PlatformMinGapX = 0.7f;     // Maximum horizontal distance between 2 platforms
    const float k_PlatformMaxGapY = 2.5f;     // Maximum vertical distance between 2 platforms
    const float k_PlatformMinGapY = 1.0f;     // Minimum vertical distance between 2 platforms

    float m_PlatformCurrentX;       // X coordinate of currently spawned platform
    float m_PlatformCurrentY;       // Y coordinate of currently spawned platform

    float m_PlatformPreviousX;      // X coordinate of previously spawned platform
    float m_PlatformPreviousY;      // Y coordinate of previously spawned platform

    bool m_BuildingRight = false;       // Boolean to decide if platforms path should follow in the right or left direction with the current group 

    float m_GroupCountInDirection;          // Number of platforms following in a direction
    float m_PlatformsInCurrentGroup;        // Number of platforms in current group

    public GameObject platformPrefabLH;       // Prefab of the large horizontal platform  to spawn
    public GameObject platformPrefabMH;       // Prefab of the medium horizontal platform to spawn
    public GameObject platformPrefabSH;       // Prefab of the small horizontal platform to spawn

    public GameObject platformPrefabLV;       // Prefab of the large vertical platform  to spawn
    public GameObject platformPrefabMV;       // Prefab of the medium vertical platform to spawn
    public GameObject platformPrefabSV;       // Prefab of the small vertical platform to spawn

    public GameObject platformPrefabMHMoving; // Prefab of the medium MOVING horizontal platform to spawn
    public GameObject platformPrefabMVMoving; // Prefab of the medium MOVING vertical platform to spawn

    public GameObject flyPrefab;

    public GameObject colliderToSpawnMore;


    public struct Platforms
    {
        public float k_GapX;
        public float k_GapY;
        public GameObject k_PlatformPrefab;

        public Platforms(float gapX, float gapY, GameObject PlatformPrefab)
        {
            this.k_GapX = gapX;
            this.k_GapY = gapY;
            this.k_PlatformPrefab = PlatformPrefab;
        }
    }

    

    List <Platforms> platformList;


    // Start is called before the first frame update
    void Start()
    {
        Platforms LongHorizontal = new Platforms(2f, 0f, platformPrefabLH);
        Platforms MediumHorizontal = new Platforms(1.5f, 0f, platformPrefabMH);
        Platforms SmallHorizontal = new Platforms(1f, 0f, platformPrefabSH);
        Platforms MediumHorizontalMoving = new Platforms(2f, 0f, platformPrefabMHMoving);
        Platforms LongVertical = new Platforms(0f, 2.5f, platformPrefabLV);
        Platforms MediumVertical = new Platforms(0f, 2f, platformPrefabMV);
        Platforms SmallVertical = new Platforms(0f, .5f, platformPrefabSV);
        Platforms MediumVerticalMoving = new Platforms(0f, 3f, platformPrefabMVMoving);

        m_CameraHeight = 2f * Camera.main.orthographicSize;
        m_CameraWidth = m_CameraHeight * Camera.main.aspect;
        //Debug.Log(m_CameraHeight + " " + m_CameraWidth);
        k_PlatformMinX = -1f * m_CameraWidth / 2f * 0.75f;
        k_PlatformMaxX = m_CameraWidth / 2f * 0.75f;
        //Debug.Log(k_PlatformMinX + " " + k_PlatformMaxX);

        m_PlatformPreviousX = Random.Range(k_PlatformMinX, k_PlatformMaxX);  
        m_PlatformPreviousY = 0;

        platformList = new List<Platforms>
        {
            LongHorizontal,
            MediumHorizontal,
            SmallHorizontal,
            MediumHorizontalMoving,
            LongVertical,
            MediumVertical,
            SmallVertical,
            MediumVerticalMoving
        };

        CalculatePlatformPosition();
    }


    void CalculatePlatformPosition()
    {
        // Generate random platforms

        m_PlatformsInCurrentGroup = 0;
        int minHorizontalsGroup = 5;
        int platformIndex;
        bool checkerBuild = false;
        int platformCountFly = 0;

        while (m_PlatformsInCurrentGroup < 50)
        {
            if (m_PlatformsInCurrentGroup > m_PlatformsInCurrentGroup / 2 && !checkerBuild)
            {
                GameObject colliderChecker = Instantiate(colliderToSpawnMore, new Vector2(0, m_PlatformPreviousY), Quaternion.identity);
                StartCoroutine(CheckSpawnMore(colliderChecker));
                checkerBuild = true;
            }
            // group of platforms going in one direction randomly between 2 to 6
            int platformCount = Random.Range(2, 6);
            m_BuildingRight = !m_BuildingRight;

            for (int i = 0; i < platformCount; i++)
            {
                if (minHorizontalsGroup-- > 0)
                {
                    //Debug.Log(platformList.Count / 2);
                    platformIndex = Random.Range(0, platformList.Count/2);
                } 
                else
                {
                    platformIndex = Random.Range(0, platformList.Count);
                    if (platformIndex >= 4)
                    {
                        minHorizontalsGroup = 5;
                        if (i < platformCount - 1)
                            m_BuildingRight = !m_BuildingRight;
                    }
                }
                


                // Y gap randomly from min to max


                m_PlatformCurrentY = m_PlatformPreviousY + Random.Range(k_PlatformMinGapY, k_PlatformMaxGapY) + platformList[platformIndex].k_GapY;


                // X gap: either move them to right or to left by random gap (min to max)
                if (m_BuildingRight)
                {
                    // to the right
                    m_PlatformCurrentX = m_PlatformPreviousX + Random.Range(k_PlatformMinGapX, k_PlatformMaxGapX) + platformList[platformIndex].k_GapX;
                    if (m_PlatformCurrentX > k_PlatformMaxX)
                    {
                        m_PlatformCurrentX = k_PlatformMaxX;
                    }
                }
                else
                {
                    // to the left
                    m_PlatformCurrentX = m_PlatformPreviousX - Random.Range(k_PlatformMinGapX, k_PlatformMaxGapX) - platformList[platformIndex].k_GapX;
                    if (m_PlatformCurrentX < k_PlatformMinX)
                    {
                        m_PlatformCurrentX = k_PlatformMinX;
                    }
                }

                
                //save previous platform coordinates for future work. TODO: make this struct
                m_PlatformPreviousY = m_PlatformCurrentY;
                m_PlatformPreviousX = m_PlatformCurrentX;


                //Generate position for new platform and instantiate. Add to platform counter.
                Vector2 position = new Vector2(m_PlatformCurrentX, m_PlatformCurrentY);

                Instantiate(platformList[platformIndex].k_PlatformPrefab, new Vector2(position.x, position.y), Quaternion.identity);
                m_PlatformsInCurrentGroup++;
                if (platformCountFly++ == 15)
                {
                    Instantiate(flyPrefab, new Vector2(position.x + 0.3f , position.y + 1.5f), Quaternion.identity);
                    platformCountFly = 0;
                }
            }
        }
    }

    private IEnumerator CheckSpawnMore(GameObject colliderToCheck)
    {
        PlatformAddMoreCheck script = colliderToCheck.GetComponent<PlatformAddMoreCheck>();
        while (!script.m_ColliderTriggered)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(colliderToCheck);
        CalculatePlatformPosition();
        

        yield return null;
    }
}
