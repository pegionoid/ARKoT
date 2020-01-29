namespace ARKoT
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    public class ARKoTController : MonoBehaviour
    {
        /// <summary>
        /// A prefab for visualizing an AugmentedImage.
        /// </summary>
        public TokyoVisualizer TokyoVisualizerPrefab;
        public MonstorVisualizer MonstorVisualizerPrefab;
        
        private TokyoVisualizer m_tokyoVisualizer;

        private Dictionary<string, MonstorVisualizer> m_monstorVisualizers
            = new Dictionary<string, MonstorVisualizer>();

        private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

        private MonstorVisualizer turnMonstor = null;

        public Transform TokyoTransform
        {
            get { return m_tokyoVisualizer.transform; }
        }

        //public bool isTokyo(MonstorVisualizer monstor)
        //{
        //    bool res = false;

        //    if (monstor == null) return res;
        //    if (m_tokyoVisualizer == null) return res;

        //    float halfHeight = monstor.Image.ExtentZ / 2;
        //    Vector3 monstorPosition = monstor.Image.CenterPose.position + (halfHeight * Vector3.back) ;


            




        //}

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Enable ARCore to target 60fps camera capture frame rate on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;

        }

        // Update is called once per frame
        void Update()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            // Get updated augmented images for this frame.
            Session.GetTrackables<AugmentedImage>(
                m_TempAugmentedImages, TrackableQueryFilter.Updated);
            //Debug.Log($"[Debug@Controller]TrackablesCount:{m_TempAugmentedImages.Count}");
            // Create visualizers and anchors for updated augmented images that are tracking and do
            // not previously have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempAugmentedImages)
            {
                //Debug.Log($"[Debug@Controller]{image.Name}:{image.CenterPose}");
                switch (image.Name)
                {
                    case "TokyoBoard":
                        if (image.TrackingState == TrackingState.Tracking && m_tokyoVisualizer == null)
                        {
                            // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                            Anchor anchor = image.CreateAnchor(image.CenterPose);
                            m_tokyoVisualizer = (TokyoVisualizer)Instantiate(
                            TokyoVisualizerPrefab, anchor.transform);
                            m_tokyoVisualizer.Image = image;

                            foreach(MonstorVisualizer m in m_monstorVisualizers.Values)
                            {
                                m.transform.parent = m_tokyoVisualizer.transform;
                            }
                        }
                        else if (image.TrackingState == TrackingState.Stopped && m_tokyoVisualizer != null)
                        {
                            foreach (MonstorVisualizer m in m_monstorVisualizers.Values)
                            {
                                m.transform.parent = null;
                            }
                            GameObject.Destroy(m_tokyoVisualizer.gameObject);
                        }

                        break;

                    default:
                        MonstorVisualizer visualizer = null;
                        string[] monstorName = image.Name.Split('_');
                        m_monstorVisualizers.TryGetValue(monstorName[0], out visualizer);
                        if (image.TrackingState == TrackingState.Tracking && image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
                        {
                            if (visualizer == null)
                            {
                                visualizer = (MonstorVisualizer)Instantiate(
                                MonstorVisualizerPrefab);

                                visualizer.Image = image;

                                m_monstorVisualizers.Add(monstorName[0], visualizer);
                            }
                            else if(!(visualizer.Contains(image)))
                            {
                                visualizer.Image = image;
                            }
                            
                        }
                        else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
                        {
                            m_monstorVisualizers.Remove(image.Name);
                            GameObject.Destroy(visualizer.gameObject);
                        }
                        break;
                }                
            }
        }
    }
}