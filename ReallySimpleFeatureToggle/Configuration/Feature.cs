﻿using System;
using System.Collections.Generic;
using ReallySimpleFeatureToggle.AvailabilityRules;
using ReallySimpleFeatureToggle.FeatureStateEvaluation;

namespace ReallySimpleFeatureToggle.Configuration
{
    public class Feature : IFeatureWithFluentExtensions
    {
        public Feature(Enum feature) : this(feature.ToString())
        {
        }

        public Feature(string name)
        {
            Name = name;
            AdditionalRules = new List<IAvailabilityRule>();
        }

        public string Name { get; set; }

        public State State { get; set; }
        public int RandomPercentageEnabled { get; set; }
        public IList<IAvailabilityRule> AdditionalRules { get; set; } 

        private string[] _dependencies;
        public string[] Dependencies
        {
            get { return _dependencies ?? new string[0]; }
            set { _dependencies = value; }
        }

        private string[] _supportedTenants;
        public string[] SupportedTenants
        {
            get
            {
                return (_supportedTenants == null || _supportedTenants.Length == 0)
                           ? new[] {Tenant.All}
                           : _supportedTenants;
            }
            set { _supportedTenants = value; }
        }

        private string[] _excludedTenants;
        public string[] ExcludedTenants
        {
            get
            {
                return (_excludedTenants == null || _excludedTenants.Length == 0)
                           ? new string[0]
                           : _excludedTenants;
            }
            set { _excludedTenants = value; }
        }
        
        public DateTime StartDtg { get; set; }

        private DateTime _endDtg;
        public DateTime EndDtg
        {
            get { return _endDtg == DateTime.MinValue ? DateTime.MaxValue : _endDtg; }
            set { _endDtg = value; }
        }

        public IFeatureWithFluentExtensions ThatIsEnabled()
        {
            State = State.Enabled;
            return this;
        }

        public IFeatureWithFluentExtensions ThatIsDisabled()
        {
            State = State.Disabled;
            return this;
        }

        public IFeatureWithFluentExtensions EnabledForPercentage(int percentage)
        {
            State = State.EnabledForPercentage;
            RandomPercentageEnabled = percentage;
            return this;
        }

        public IFeatureWithFluentExtensions OnlyAvailableBetween(DateTime startTime, DateTime endTime)
        {
            StartDtg = startTime;
            EndDtg = endTime;
            return this;
        }

        public IFeatureWithFluentExtensions WithCustomAvailabilityRule(Func<IFeature, EvaluationContext, bool> evaluation)
        {
            var customRule = new AvailabilityRuleShim(evaluation);
            AdditionalRules.Add(customRule);
            return this;
        }

        public IFeatureWithFluentExtensions WithCustomAvailabilityRule(IAvailabilityRule rule)
        {
            AdditionalRules.Add(rule);
            return this;
        }

        public static IFeatureWithFluentExtensions Called(string name)
        {
            return new Feature(name);
        }
    }
}