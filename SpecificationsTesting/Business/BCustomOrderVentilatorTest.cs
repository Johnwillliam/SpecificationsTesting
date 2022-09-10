﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EntityFrameworkModel;

namespace SpecificationsTesting.Business
{
    public static class BCustomOrderVentilatorTest
    {
        protected static readonly List<string> controleDisplayPropertyNames = new List<string>
        {
            "MeasuredVentilatorHighRPM", "MeasuredVentilatorLowRPM", "MeasuredMotorHighRPM", "MeasuredMotorLowRPM", "MeasuredBladeAngle", "Cover",
            "I1High", "I1Low", "I2High", "I2Low", "I3High", "I3Low", "MotorNumber", "Weight", "Date", "UserID", "MotorNumber", "BuildSize"
        };

        public static List<string> ControleDisplayPropertyNames => controleDisplayPropertyNames;

        public static CustomOrderVentilatorTest Create(CustomOrderVentilatorTest customOrderVentilatorTest)
        {
            using (var dbContext = new SpecificationsDatabaseModel())
            {
                dbContext.CustomOrderVentilatorTests.Add(customOrderVentilatorTest);
                dbContext.SaveChanges();
                return customOrderVentilatorTest;
            }
        }

        public static void Create(CustomOrder customOrder)
        {
            foreach (var ventilator in customOrder.CustomOrderVentilators)
            {
                for (int i = 0; i < ventilator.Amount; i++)
                {
                    Create(new CustomOrderVentilatorTest() { CustomOrderVentilatorID = ventilator.ID });
                }
            }
        }

        public static void Create(CustomOrderVentilator customOrderVentilator)
        {
            for (int i = 0; i < customOrderVentilator.Amount; i++)
            {
                Create(new CustomOrderVentilatorTest() { CustomOrderVentilatorID = customOrderVentilator.ID });
            }
        }

        public static void Update(CustomOrderVentilatorTest customOrderVentilatorTest)
        {
            using (var dbContext = new SpecificationsDatabaseModel())
            {
                var toUpdate = dbContext.CustomOrderVentilatorTests.Find(customOrderVentilatorTest.ID);
                if (toUpdate != null)
                {
                    dbContext.Entry(toUpdate).CurrentValues.SetValues(customOrderVentilatorTest);
                    dbContext.SaveChanges();
                    Thread.Sleep(300);
                }
            }
        }

        public static string ValidateForPrinting(CustomOrderVentilatorTest test)
        {
            if (test.MeasuredBladeAngle == null)
            {
                return "Measured blade angle not filled in.";
            }
            if (test.MeasuredVentilatorHighRPM == null)
            {
                return "Measured ventilator high RPM not filled in.";
            }
            if (test.MeasuredVentilatorLowRPM == null)
            {
                return "Measured ventilator low RPM not filled in.";
            }
            if (test.MeasuredMotorHighRPM == null)
            {
                return "Measured motor high RPM not filled in.";
            }
            if (test.MeasuredMotorLowRPM == null)
            {
                return "Measured motor low RPM not filled in.";
            }
            if (test.MeasuredBladeAngle != test.CustomOrderVentilator.BladeAngle)
            {
                return "Measured blade angle does not matched the ordered angle.";
            }
            if (test.I1High > test.CustomOrderVentilator.CustomOrderMotor.HighAmperage || test.I2High > test.CustomOrderVentilator.CustomOrderMotor.HighAmperage || test.I3High > test.CustomOrderVentilator.CustomOrderMotor.HighAmperage)
            {
                return "One of the measured amperages is higher than the nominal amperage.";
            } 
            if (test.CustomOrderVentilator.CustomOrderMotor.HighRPM == null)
            {
                return "Motor high RPM is not filled in.";
            }
            if (test.CustomOrderVentilator.HighRPM == null)
            {
                return "Ventilator high RPM is not filled in.";
            }
            if (test.MeasuredMotorHighRPM == null)
            {
                return "Measured motor high RPM not filled in.";
            }
            if (test.MeasuredMotorHighRPM < test.CustomOrderVentilator.CustomOrderMotor.HighRPM)
            {
                return "The measured motor RPM is lower than the nominal RPM.";
            }
            if (test.MeasuredMotorHighRPM != null && test.CustomOrderVentilator.CustomOrderMotor.Frequency != null)
            {
                var syncRPM = CalculateSyncRPM(test.MeasuredMotorHighRPM.Value, test.CustomOrderVentilator.CustomOrderMotor.Frequency.Value);
                if (test.MeasuredMotorHighRPM > syncRPM)
                {
                    return $"Measured motor high RPM ({test.MeasuredMotorHighRPM}) is higher than the synchronous rpm ({syncRPM}). This is not possible.";
                }
                if (test.MeasuredVentilatorHighRPM > test.CustomOrderVentilator.CustomOrderMotor.HighRPM)
                {
                    return $"Measured ventilator high RPM ({test.MeasuredVentilatorHighRPM}) is higher than the motor RPM ({test.CustomOrderVentilator.CustomOrderMotor.HighRPM}), wrong motor?";
                }
            }
            if (test.MeasuredMotorLowRPM != null && test.CustomOrderVentilator.CustomOrderMotor.Frequency != null)
            {
                var syncRPM = CalculateSyncRPM(test.MeasuredMotorLowRPM.Value, test.CustomOrderVentilator.CustomOrderMotor.Frequency.Value);
                if (test.MeasuredMotorLowRPM > syncRPM)
                {
                    return $"Measured motor low RPM ({test.MeasuredMotorLowRPM}) is higher than the synchronous rpm ({syncRPM}). This is not possible.";
                }
                if (test.MeasuredVentilatorHighRPM > test.CustomOrderVentilator.CustomOrderMotor.HighRPM)
                {
                    return $"Measured ventilator high RPM ({test.MeasuredVentilatorHighRPM}) is higher than the motor RPM ({test.CustomOrderVentilator.CustomOrderMotor.HighRPM}), wrong motor?";
                }
            }
            if (test.MeasuredVentilatorHighRPM != null && 
                MeasuredVentilatorRPMIsInSpec(test.CustomOrderVentilator.CustomOrderMotor.HighRPM, test.CustomOrderVentilator.HighRPM, test.MeasuredMotorHighRPM, test.MeasuredVentilatorHighRPM))
            {
                return "The measured ventilator high RPM differs more than 3%.";
            }
            return string.Empty;
        }

        public static bool MeasuredVentilatorRPMIsInSpec(int? customOrderMotorHighRPM, int? customOrderVentilatorHighRPM, int? measuredMotorHighRPM, int? measuredVentilatorHighRPM)
        {
            var nv = measuredMotorHighRPM / customOrderMotorHighRPM * customOrderVentilatorHighRPM;
            return Math.Max((double)customOrderVentilatorHighRPM, (double)measuredVentilatorHighRPM) / Math.Min((double)nv, (double)measuredVentilatorHighRPM) > 1.03;
        }

        public static int CalculateSyncRPM(int measuredMotorHighRPM, int frequency)
        {
            var syncRPM = (double)measuredMotorHighRPM / (double)frequency;

            switch (syncRPM)
            {
                case double n when (n <= 10):
                    return frequency * 10;
                case double n when (n <= 15):
                    return frequency * 15;
                case double n when (n <= 20):
                    return frequency * 20;
                case double n when (n <= 30):
                    return frequency * 30;
                case double n when (n <= 60):
                    return frequency * 60;
            }
            return 0;
        }
    }
}
