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
        

        public bool IsTokyo
        {
            get;
            private set;
        }

        public float Height
        {
            get;
            private set;
        }

        public float Width
        {
            get;
            private set;
        }

        private GameObject MonstorModel;
        private Animator MonstorAnimation;

        /// <summary>
        /// The AugmentedImage to visualize.
        /// </summary>
        public AugmentedImage Image
        {
            set
            {
                _Images.Add(value);
                //Debug.Log($"[Debug@Monstor]ImageName:{value.Name}; ExtentSize({value.ExtentX},{value.ExtentZ})");

                if (_Images.Count == 1)
                {
                    AugmentedImage image = _Images[0];
                    this.Height = image.ExtentZ;
                    this.Width = image.ExtentX;
                    halfHeight = Height / 2;
                    string[] monstorName = image.Name.Split('_');

                    GameObject front = transform.Find("Front").gameObject;
                    GameObject back = transform.Find("Back").gameObject;

                    front.transform.localScale = new Vector3(image.ExtentX * 0.1f, 0, image.ExtentZ * 0.1f);
                    back.transform.localScale = new Vector3(image.ExtentX * 0.1f, 0, image.ExtentZ * 0.1f);

                    Material material;
                    Texture2D covertex = (Texture2D)Resources.Load($"Images/{monstorName[0]}_Cover");
                    //Debug.Log($"[Debug@Monstor]TextureLoad:{covertex != null}");

                    if (covertex == null)
                    {
                        material = (Material)Resources.Load($"Assets/Material/Clear");
                    }
                    else
                    {
                        material = new Material(Shader.Find("Unlit/Transparent"));
                        material.mainTexture = covertex;

                        //Debug.Log($"[Debug@Monstor]Material:{material.shader.name}");
                        //Debug.Log($"[Debug@Monstor]Texture:{material.mainTexture.name}");
                    }
                    front.GetComponent<Renderer>().material = material;
                    front.GetComponent<Renderer>().material.mainTextureScale = new Vector2(-1, -1);
                    back.GetComponent<Renderer>().material = material;
                    back.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, -1);

                    MonstorModel = (GameObject)Instantiate(Resources.Load($"Prefabs/{monstorName[0]}"), this.transform.position, this.transform.rotation);
                    MonstorAnimation = MonstorModel.GetComponent<Animator>();
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


            //Debug.Log($"[Debug@Monstor]MonstoeModel.Position:{MonstorModel.transform.position}");
            //Debug.Log($"[Debug@Monstor]MonstoeModel.Rotation:{MonstorModel.transform.rotation}");

            return;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[Debug@Monstor]OnTriggerEnter:{other.name}");
            switch (other.name)
            {
                case "TOKYOCITY":
                case "TOKYOBAY":
                    IsTokyo = true;
                    MonstorAnimation.SetBool("IsTokyo", IsTokyo);
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
                case "TOKYOBAY":
                    IsTokyo = false;
                    MonstorAnimation.SetBool("IsTokyo", IsTokyo);
                    break;

                default:
                    break;
            }
        }

        //public IEnumerator Attack(int count)
        //{
        //    Debug.Log($"[Debug@Monstor]Attack{count}");
        //    MonstorAnimation.SetInteger("Attack", count);
        //    yield return null;
        //    yield return new WaitForAction(MonstorAnimation, "Attack");
        //}

        public void Attack(int count)
        {
            Debug.Log($"[Debug@Monstor]Attack{count}");
            MonstorAnimation.SetInteger("Attack", count);
        }

        public void Heal(int count)
        {
            if(!IsTokyo)
            {
                Debug.Log($"[Debug@Monstor]Heal{count}");
                MonstorAnimation.SetInteger("Heal", count);
            }
        }

        public void Charge(int count)
        {
            Debug.Log($"[Debug@Monstor]Charge{count}");
            MonstorAnimation.SetInteger("Charge", count);
        }

        public void Damage(int count)
        {
            Debug.Log($"[Debug@Monstor]Damage{count}");
            MonstorAnimation.SetTrigger("Damage");
        }

        public void Destruct(int count)
        {
            Debug.Log($"[Debug@Monstor]Destruct{count}");
            MonstorAnimation.SetInteger("Destruct", count);
        }

        private AugmentedImage getFullTrackingImage()
        {
            AugmentedImage res = _Images.Find(i => i.TrackingMethod == AugmentedImageTrackingMethod.FullTracking);
            if (res == null) res = lastTrackedImage;

            return res;
        }
    }

    public class WaitForAction : CustomYieldInstruction
    {
        Animator m_animator;
        string m_property;

        public WaitForAction(Animator animator, string property)
        {
            m_animator = animator;
            m_property = property;
        }

        public override bool keepWaiting
        {
            get
            {
                int currentPropertyValue = m_animator.GetInteger(m_property);
                return currentPropertyValue > 0;
            }
        }
    }
}

