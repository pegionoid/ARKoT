namespace ARKoT
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

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
        private GameObject cursorTurnMonstor = null;

        public Transform TokyoTransform
        {
            get { return m_tokyoVisualizer == null ? null : m_tokyoVisualizer.transform; }
        }

        public bool SwitchTurnMonstor(eMonstors monstor)
        {
            MonstorVisualizer visualizer = null;
            m_monstorVisualizers.TryGetValue(monstor.ToString(), out visualizer);
            if (visualizer == null) return false;

            this.turnMonstor = visualizer;
            if (cursorTurnMonstor == null)
            {
                cursorTurnMonstor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cursorTurnMonstor.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }

            cursorTurnMonstor.transform.parent = visualizer.transform;
            cursorTurnMonstor.transform.localPosition = (visualizer.Height/2 + 0.02f) * Vector3.forward;

            return true;            
        }

        public void DiceResolution(eMonstors monstor, List<eDiceEye> diceEyes)
        {
            MonstorVisualizer visualizer = null;
            m_monstorVisualizers.TryGetValue(monstor.ToString(), out visualizer);
            if (visualizer == null) return;
            
            while (diceEyes.Count > 0)
            {
                int count = 0;
                switch (diceEyes[0])
                {
                    case eDiceEye.Attack:
                        count = diceEyes.FindAll(d => d == eDiceEye.Attack).Count;
                        visualizer.Attack(count);
                        if(visualizer.IsTokyo)
                        {
                            foreach (MonstorVisualizer mv in m_monstorVisualizers.Values.Where(m => !m.Equals(visualizer)))
                            {
                                mv.Damage(count);
                            }
                        }
                        else
                        {
                            foreach(MonstorVisualizer mv in m_monstorVisualizers.Values.Where(m => m.IsTokyo))
                            {
                                mv.Damage(count);
                            }
                        }
                        diceEyes.RemoveAll(d => d == eDiceEye.Attack);

                        break;

                    case eDiceEye.Energy:
                        count = diceEyes.FindAll(d => d == eDiceEye.Energy).Count;
                        visualizer.Charge(count);
                        diceEyes.RemoveAll(d => d == eDiceEye.Energy);
                        break;

                    case eDiceEye.Heal:
                        count = diceEyes.FindAll(d => d == eDiceEye.Heal).Count;
                        if(!visualizer.IsTokyo) visualizer.Heal(count);
                        diceEyes.RemoveAll(d => d == eDiceEye.Heal);
                        break;

                    case eDiceEye.OnePoint:
                    case eDiceEye.TwoPoint:
                    case eDiceEye.ThreePoint:
                        int point = 0;
                        count = diceEyes.FindAll(d => d == eDiceEye.OnePoint).Count - 3;
                        point += (count >= 0 ? 1 + count : 0);

                        count = diceEyes.FindAll(d => d == eDiceEye.TwoPoint).Count - 3;
                        point += (count >= 0 ? 2 + count : 0);

                        count = diceEyes.FindAll(d => d == eDiceEye.ThreePoint).Count - 3;
                        point += (count >= 0 ? 3 + count : 0);

                        if (point > 0) visualizer.Destruct(point);
                        diceEyes.RemoveAll(d => d == eDiceEye.OnePoint);
                        diceEyes.RemoveAll(d => d == eDiceEye.TwoPoint);
                        diceEyes.RemoveAll(d => d == eDiceEye.ThreePoint);
                        break;
                }
            }
        }
        
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