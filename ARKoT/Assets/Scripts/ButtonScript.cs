namespace ARKoT
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ButtonScript : MonoBehaviour
    {
        ARKoTController ARKoTController;
        Dropdown monstor;
        List<eDiceEye> eDiceEyes = new List<eDiceEye>();
        public void Awake()
        {
            ARKoTController = GameObject.Find("Manager").GetComponent<ARKoTController>();
            monstor = GameObject.Find("Dropdown_Monstor").GetComponent<Dropdown>();

            //EnumをStringの配列に変換
            string[] enumNames = System.Enum.GetNames(typeof(eMonstors));

            //Stringの配列をリストに変換
            List<string> names = new List<string>(enumNames);

            //DropDownの要素にリストを追加
            monstor.AddOptions(names);
        }

        public void OnClick(int number)
        {
            switch(number)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    eDiceEyes.Add((eDiceEye)number);
                    break;

                case 6:
                    ARKoTController.DiceResolution((eMonstors)monstor.value, eDiceEyes);
                    eDiceEyes.Clear();
                    break;
            }
        }
    }
}

