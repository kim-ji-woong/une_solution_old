using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    [Serializable]
    public class Poi
    {

        public string Id = "";
        public string PoiName = "";
        public string PoiTypeName ="";
        public float X ;
        public float Y ;
        public float Z ;

        public bool Activate = false;

        public bool Visible = true ;
    }
}
