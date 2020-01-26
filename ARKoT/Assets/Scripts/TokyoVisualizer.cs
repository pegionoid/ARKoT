namespace ARKoT
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GoogleARCore;
    using GoogleARCoreInternal;

    public class TokyoVisualizer : MonoBehaviour
    {
        /// <summary>
        /// The AugmentedImage to visualize.
        /// </summary>
        public AugmentedImage Image;

        /// <summary>
        /// A model for when an image is detected.
        /// </summary>
        private GameObject Model;
        
        // Update is called once per frame
        public void Update()
        {
            if (Image == null || Image.TrackingState != TrackingState.Tracking)
            {
                if (Model != null) Model.SetActive(false);
                return;
            }

            if (Model != null) Model.SetActive(true);
        }
    }
}

