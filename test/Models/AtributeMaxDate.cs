using System;
using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class AtributeMaxDate: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                if (value != null)
                {
                    DateTime dateValue = (DateTime)value;

                    if (dateValue >= DateTime.Now)
                    {
                        return false;
                    }

                    return true;
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