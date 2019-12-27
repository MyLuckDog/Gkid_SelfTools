using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MyTools;
namespace Windows
{
    [UUIResources("MyPanel")]
    partial class MyPanel : UUIAutoGenWindow
    {
        protected Text Text;
        protected RawImage RawImage;
        protected RawImage RawImagefse;

        protected override void InitTemplate()
        {
            base.InitTemplate();
            Text = FindChild<Text>("Text");
            RawImage = FindChild<RawImage>("RawImage");
            RawImagefse = FindChild<RawImage>("RawImagefse");

           
        }
    }
}
