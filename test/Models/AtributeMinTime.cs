using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace test.Models
{
    public class AtributeMinTime : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                const int minValue = 5;
                var maxValue = Convert.ToInt32(ConfigurationManager.AppSettings["maxTimeValue"]);

                if (value != null)
                {
                    var inputValue = (int)value;

                    if (inputValue >= minValue && inputValue <= maxValue)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }

            return false;
        }
    }
}