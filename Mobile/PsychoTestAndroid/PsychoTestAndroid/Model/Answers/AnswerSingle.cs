﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PsychoTestAndroid.Model.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsychoTestAndroid.Model.Answers
{
    public class AnswerSingle : Answer
    {
        [JsonIgnore]
        protected RadioButton radio;

        public AnswerSingle() : base()
        {

        }

        public AnswerSingle(JObject data) : base(data)
        {

        }

        public override LinearLayout Show(LinearLayout layout)
        {
            layout.Orientation = Orientation.Horizontal;
            layout.SetGravity(GravityFlags.CenterVertical);
            radio = new RadioButton(layout.Context);
            radio.Click += Select;
            layout.Click += Select;
            layout.AddView(radio);
            radio.LayoutParameters.Width = ViewGroup.LayoutParams.WrapContent;
            radio.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
            UpdateResult(owner.result);
            return base.Show(layout);
        }

        public override void UpdateResult(string result)
        {
            if (radio != null)
            {
                radio.Checked = result == Id;
            }
        }

        protected void Select(object sender, EventArgs e)
        {
            radio.Checked = true;
            if (radio.Checked)
            {
                owner.SetResult(Id);
            }
        }
    }
}