namespace ARKoT
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GoogleARCore;
    using GoogleARCoreInternal;

    public class MonstorVisualizer : MonoBehaviour
    {
        
        private List<AugmentedImage> _Images = new List<AugmentedImage>();
        private AugmentedImage lastTrackedImage;

        /// <summary>
        /// A model for when an image is detected.
        /// </summary>
        private GameObject Model;
        private float halfHeight;

        private Collider Collider;

        public bool IsTokyo
        {
            get;
            private set;
        }

        private GameObject MonstorModel;

        /// <summary>
        /// The AugmentedImage to visualize.
        /// </summary>
        public AugmentedImage Image
        {
            set
            {
                _Images.Add(value);
                Debug.Log($"[Debug@Monstor]ImageName:{value.Name}; ExtentSize({value.ExtentX},{value.ExtentZ})");

                if(_Images.Count == 1)
                {
                    AugmentedImage image = _Images[0];
                    halfHeight = image.ExtentZ / 2;
                    string[] monstorName = image.Name.Split('_');

                    GameObject front = transform.Find("Front").gameObject;
                    GameObject back = transform.Find("Back").gameObject;

                    front.transform.localScale = new Vector3(image.ExtentX * 0.1f, 0, image.ExtentZ * 0.1f);
                    back.transform.localScale = new Vector3(image.ExtentX * 0.1f, 0, image.ExtentZ * 0.1f);

                    Material material;
                    Texture2D covertex = (Texture2D)Resources.Load($"Images/Cover_{monstorName[0]}");
                    //Debug.Log($"[Debug@Monstor]TextureLoad:{covertex != null}");

                    if (covertex == null)
                    {
                        material = (Material)Resources.Load($"Assets/Material/Clear");
                    }
                    else
                    {
                        material = new Material(Shader.Find("Unlit/Transparent"));
                        material.mainTexture = covertex;

                        Debug.Log($"[Debug@Monstor]Material:{material.shader.name}");
                        Debug.Log($"[Debug@Monstor]Texture:{material.mainTexture.name}");
                    }
                    front.GetComponent<Renderer>().material = material;
                    front.GetComponent<Renderer>().material.mainTextureScale = new Vector2(-1, -1);
                    back.GetComponent<Renderer>().material = material;
                    back.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, -1);

                    MonstorModel = (GameObject)Instantiate(Resources.Load($"Prefabs/giant"), this.transform.position, this.transform.rotation);
                }
            }
        }
        public AugmentedImage FrontImage
        {
            get
            {
                return _Images.Find(i => i.Name.EndsWith("Front"));
            }
            
            set
            {
                Image = value;
            }
        }

        public AugmentedImage BackImage
        {
            get
            {
                return _Images.Find(i => i.Name.EndsWith("Back"));
            }

            set
            {
                Image = value;
            }
        }
        
        public bool Contains(AugmentedImage image)
        {
            return _Images.Contains(image);
        }

        // Update is called once per frame
        public void Update()
        {
            if (_Images.Count <= 0 || !(_Images.Exists(i => i.TrackingState == TrackingState.Tracking)))
            {
                foreach (Transform childTransform in transform)
                {
                    Debug.Log(childTransform.gameObject.name);
                    childTransform.gameObject.SetActive(false);
                }
                return;
            }

            lastTrackedImage = getFullTrackingImage();
            Vector3 pos = lastTrackedImage.CenterPose.position + (0.0005f * Vector3.down);
            Quaternion rot = lastTrackedImage.CenterPose.rotation;
            if(lastTrackedImage.Name.EndsWith("Back")) rot *= Quaternion.Euler(0f, 0f, 180.0f);
            this.transform.SetPositionAndRotation(pos, rot);

            foreach (Transform childTransform in transform)
            {
                Debug.Log(childTransform.gameObject.name);
                childTransform.gameObject.SetActive(true);
            }

            MonstorModel.transform.position = this.transform.position + (halfHeight * -this.transform.forward) + (0.02f * this.transform.up);
            MonstorModel.transform.rotation = this.transform.rotation * Quaternion.Euler(90f, 0f, 0f) * Quaternion.Euler(0f, 180f, 0f);


            Debug.Log($"[Debug@Monstor]MonstoeModel.Position:{MonstorModel.transform.position}");
            Debug.Log($"[Debug@Monstor]MonstoeModel.Rotation:{MonstorModel.transform.rotation}");

            return;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[Debug@Monstor]OnTriggerEnter:{other.name}");
            switch (other.name)
            {
                case "TOKYOCITY":
                    IsTokyo = true;
                    break;

                case "TOKYOBAY":
                    IsTokyo = true;
                    break;

                default:
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log($"[Debug@Monstor]OnTriggerExit:{other.name}");
            switch (other.name)
            {
                case "TOKYOCITY":
                    IsTokyo = false;
                    break;

                case "TOKYOBAY":
                    IsTokyo = false;
                    break;

                default:
                    break;
            }
        }

        public void Attack()
        {

        }

        public void Healing()
        {

        }

        public void Charge()
        {

        }

        public void Damage()
        {

        }

        private AugmentedImage getFullTrackingImage()
        {
            AugmentedImage res = _Images.Find(i => i.TrackingMethod == AugmentedImageTrackingMethod.FullTracking);
            if (res == null) res = lastTrackedImage;

            return res;
        }
        
    }
}

