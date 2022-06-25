﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PsychoTestAndroid.DataBase.Entity;
using PsychoTestAndroid.Model.Questions;
using PsychoTestAndroid.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsychoTestAndroid.Model
{
    public class Test
    {
        // продолжительность теста
        [JsonProperty("max_duration")]
        string duration;
        // оставшееся время
        int currentDuration;
        // порядок ответов (фиксирован / случаен)
        [JsonProperty("answer_order")]
        string answerOrder;
        // порядок вопросов (фиксирован / случаен)
        [JsonProperty("questions_order")]
        string questionOrder;
        // id теста
        [JsonProperty("id")]
        public string Id;
        // название теста
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("title")]
        public string Title;
        // инструкция к тесту
        [JsonProperty("instruction")]
        public string Instruction;
        // список вопросов
        [JsonIgnore]
        public List<Question> Questions = new List<Question>();

        public Test()
        {

        }

        public Test(DbTest test)
        {
            Id = test.Id;
            Name = test.Name;
            Title = test.Title;
            Instruction = test.Instruction;
            duration = test.Duration;
            answerOrder = test.AnswerOrder;
            questionOrder = test.QuestionOrder;
            if (test.Questions != null && test.Questions != "")
            {
                SetQuestions(JArray.Parse(test.Questions));
            }
        }

        public Test(JObject data)
        {
            Id = data["_id"].ToString();
            Name = data["IR"]["Name"]["#text"].ToString();
            Title = data["IR"]["Title"]?["#text"]?.ToString();
            Instruction = data["Instruction"]["#text"].ToString();
            duration = data["TestTime"].ToString();
            answerOrder = data["OrderOfAnswers"].ToString();
            questionOrder = data["QuestionsOrder"].ToString();
        }

        // запустить таймер
        public string StartTimer()
        {
            // время задано
            if (duration != "" && duration != "0")
            {
                // получаем оставшееся время в секундах
                currentDuration = Int32.Parse(duration);
                // преобразуем оставшееся время к формату [м][м]м:сс
                return GetDuration();
            }
            // возвращаем время
            return "";
        }

        // обработка тика таймера (1 секунда)
        public string TimerTick()
        {
            // время не закончилось
            if (currentDuration > 0)
            {
                currentDuration--;
                // получение нового времени
                return GetDuration();
            }
            else
            {
                return "";
            }
        }

        // преобразование времени в секундах к формату [м][м]м:сс
        public string GetDuration()
        {
            // определение числа секунд
            var seconds = (currentDuration % 60).ToString();
            // добавление 0 перед числом секунд для 0-9 секунд
            seconds = seconds.Length == 1 ? "0" + seconds : seconds;
            // возвращаем время
            return "" + currentDuration / 60 + ":" + seconds;
        }

        // заполнить вопросы из json объекта теста
        public void SetQuestions(JArray questions)
        {
            // записываем каждый вопрос
            foreach (JObject question in questions)
            {
                string type = question["Question_Type"].ToString();
                var newQuestion = QuestionHelper.GetQuestionForType(type, question);
                if (newQuestion != null)
                {
                    Questions.Add(newQuestion);
                    Questions.Last().SetAnswers(question);
                }
            }
        }
        // завершение теста
        public bool EndTest()
        {
            TestResult result = new TestResult(this);
            return WebApi.SendResult(JsonConvert.SerializeObject(result));
        }
        // подготовка теста к запуску
        public void StartTest()
        {
            // перемешиваем вопросы
            if (questionOrder == "1")
            {
                Mix(Questions);
            }
            // перемешиваем ответы
            if (answerOrder == "1")
            {
                foreach(Question question in Questions)
                {
                    Mix(question.Answers);
                }
            }
        }
        // перемешать элементы листа
        public void Mix<T>(List<T> list)
        {
            var random = new Random();
            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                var temp = list[j];
                list[j] = list[i];
                list[i] = temp;
            }
        }

        public bool CheckResults()
        {
            foreach (Question question in Questions)
            {
                if (!question.CheckResult())
                {
                    return false;
                }
            }
            return true;
        }
    }
}