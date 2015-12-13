using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tycoon

{
    /// <summary>
    ///This example uses arrays with 24 values each to modify how behaviors are evaluated at a specific time of day.
    /// This class works with Value Oriented behaviors, not with Goal Oriented behaviors, because it relies on the fact that all opportunities to satisfy needs are evaluated, not only the opportunities that can satisfy the need of the current goal.
    /// </summary>
    public class TimeSystem : MonoBehaviour
    {
        [Tooltip("The text that should display the time of day.")]
        public Text clockDisplay;
        [Tooltip("The text that should display modifiers of the need evaluations.")]
        public Text modifiers;
        [Tooltip("Determines the speed of the TimeSystem sample script.")]
        public float virtualMinutesToRealSecondsRatio = 1;
        [Tooltip("The intensity of the light will change over time of day.")]
        public Light sunlight;

        public int virtualHour { get; private set; }
        public int virtualMinute { get; private set; }
        [Tooltip("Which time of day the clock should be set to at game srart.")]
        public int startAtHour;

        public enum DataSets { SimpleTime, NachtDesFuchses }
        [Tooltip("Different datasets manipulate different needs at different rates at different times of day.")]
        public DataSets ChosenDataSet;

        private float[] lightIntensity;
        Dictionary<string, float[]> NeedModifiers;

        private void CreateSimpleTimeData()
        {
            //a value for each hour starting at midnight   
            //     0     1     2     3     4     5     6     7     8     9     10    11    12    13    14    15    16    17    18    19    20    21    22    23              
            float[] tirednessEvaluationModifiers
               = { 1.0f, 1.0f, 1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f };
            float[] hungerEvaluationModifiers
               = { 0.3f, 0.3f, 0.3f, 0.3f, 0.4f, 0.5f, 0.7f, 0.9f, 1.0f, 0.5f, 0.4f, 0.5f, 0.9f, 1.0f, 0.8f, 0.5f, 0.4f, 0.3f, 0.4f, 0.5f, 1.0f, 1.0f, 0.5f, 0.4f };
            float[] educationEvaluationModifiers
               = { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 1.0f, 0.9f, 0.7f, 0.9f, 1.0f, 1.0f, 0.9f, 0.6f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f };
            lightIntensity
                = new float[] { 0.1f, 0.12f, 0.14f, 0.16f, 0.2f, 0.35f, 0.5f, 0.7f, 0.9f, 1.0f, 1.0f, 1.0f, 1.1f, 1.0f, 1.0f, 1.0f, 1.0f, 0.9f, 0.8f, 0.6f, 0.4f, 0.2f, 0.1f, 0.1f };
            NeedModifiers.Add("Sleep", tirednessEvaluationModifiers);
            NeedModifiers.Add("Hunger", hungerEvaluationModifiers);
            NeedModifiers.Add("Education", educationEvaluationModifiers);
        }

        private void CreateNachtDesFuchsesData()
        {
            //a value for each hour starting at midnight   
            //     0     1     2     3     4     5     6     7     8     9     10    11    12    13    14    15    16    17    18    19    20    21    22    23           
            float[] tirednessEvaluationModifiers
               = { 1.0f, 1.0f, 1.0f, 0.9f, 0.6f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.3f, 0.6f, 0.9f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
            float[] hungerEvaluationModifiers
               = { 0.1f, 0.1f, 0.1f, 0.3f, 0.6f, 0.9f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.9f, 0.6f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
            float[] thirstEvaluationModifiers
               = { 0.1f, 0.1f, 0.1f, 0.3f, 0.6f, 0.9f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.9f, 0.6f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
            lightIntensity
               = new float[]
                 { 0.2f, 0.2f, 0.2f, 0.2f, 0.3f, 0.6f, 0.9f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.1f, 1.0f, 1.0f, 1.0f, 0.9f, 0.6f, 0.3f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
            NeedModifiers.Add("Sleep", tirednessEvaluationModifiers);
            NeedModifiers.Add("Hunger", hungerEvaluationModifiers);
            NeedModifiers.Add("Thirst", thirstEvaluationModifiers);
        }

        // Use this for initialization
        void Start()
        {
            virtualHour = startAtHour;
            virtualMinute = 0;

            // By default the simulation gives priority bonuses to needs that have lesser need satisfaction states. In these examples however we set all weights to one, so only the actual numeric satisfaction level and the modifier influence evaluation of an affordance.
            // Simulation.Manager.Instance.SetAllNeedSatisfactionWeightsToOne();

            NeedModifiers = new Dictionary<string, float[]>();

            switch (ChosenDataSet)
            {
                case DataSets.SimpleTime:
                    CreateSimpleTimeData();
                    break;
                case DataSets.NachtDesFuchses:
                    CreateNachtDesFuchsesData();
                    break;
            }

            updateWeights();

            InvokeRepeating("increaseVirtualTime", 0.0f, virtualMinutesToRealSecondsRatio);
        }

        void FixedUpdate()
        {
            string displayText = "Time of day: ";
            if (virtualHour < 10)
            {
                displayText += "0";
            }
            displayText += virtualHour.ToString() + ":";
            if (virtualMinute < 10)
            {
                displayText += "0";
            }
            displayText += virtualMinute.ToString();
            clockDisplay.text = displayText;
        }

        /// <summary>
        /// A simple simulation of a 24 hour day with a resolution on the level of minutes.
        /// </summary>
        private void increaseVirtualTime()
        {
            if (virtualMinute < 59)
            {
                virtualMinute++;
            }
            else if (virtualHour < 23)
            {
                virtualHour++;
                updateWeights();
                virtualMinute = 0;
            }
            else
            {
                virtualHour = 0;
                virtualMinute = 0;
            }
        }

        private void updateWeights()
        {
            //Adjust light - just for the show
            sunlight.intensity = lightIntensity[virtualHour];

            string currentValues = "";

            foreach (string needName in NeedModifiers.Keys)
            {
                Simulation.Manager.Instance.Data.WeightsForNeed[needName] = NeedModifiers[needName][virtualHour];
                currentValues = currentValues + " | " + needName + ": " + NeedModifiers[needName][virtualHour].ToString();
            }

            modifiers.text = "Need weights at hour " + virtualHour.ToString() + ":" + currentValues + "  |";
        }
    }
}
