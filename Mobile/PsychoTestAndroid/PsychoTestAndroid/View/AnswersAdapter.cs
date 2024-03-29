﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PsychoTestAndroid.Model.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsychoTestAndroid
{
    // Адаптер для отображения списка ответов вопроса.
    public class AnswersViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout Layout { get; set; }

        [Obsolete]
        public AnswersViewHolder(View itemview, Action<int> listener) : base(itemview)
        {
            this.IsRecyclable = false;
            Layout = itemview.FindViewById<LinearLayout>(Resource.Id.answers_recycler_item);
            itemview.Click += (sender, e) => listener(Position);
        }
    }

    public class AnswersAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        Question question;
        public AnswersAdapter(Question question)
        {
            this.question = question;
        }
        // +1, т.к. 1 элемент - пояснение к ответам.
        public override int ItemCount
        {
            get { return question.Answers.Count + 1; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AnswersViewHolder vh = holder as AnswersViewHolder;
            // Элемент на 0 позиции - пояснение к ответам.
            if (position != 0)
            {
                vh.Layout = question.Answers[position - 1].Show(vh.Layout);
            }
            else
            {
                vh.Layout.Orientation = Orientation.Horizontal;
                vh.Layout.SetGravity(GravityFlags.CenterVertical);
                TextView tx = new TextView(vh.Layout.Context);
                tx.TextSize = 20;
                vh.Layout.AddView(tx);
                tx.LayoutParameters.Width = ViewGroup.LayoutParams.MatchParent;
                tx.LayoutParameters.Height = ViewGroup.LayoutParams.WrapContent;
                if (question.Answers.Count > 1)
                {
                   tx.Text = "Вариантов ответа - " + question.Answers.Count;
                }
                else
                {
                    tx.Text = "Введите ответ:";
                }
            }
        }

        [Obsolete]
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.answers_recycler_item, parent, false);
            AnswersViewHolder vh = new AnswersViewHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}