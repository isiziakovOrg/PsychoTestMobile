﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsychoTestAndroid.Model.Answers
{
    public static class DecoratorHelper
    {
        public static AnswersDecorator GetDecorator(int type, JObject data)
        {
            switch (type)
            {
                case 0: return new TextDecorator(data);
                default: return null;
            }
        }
    }
}