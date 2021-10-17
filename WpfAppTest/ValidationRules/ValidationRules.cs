using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EditorInterface.ValidationRules
{
    class NoEmptyStringValidationRule : ValidationRule
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public NoEmptyStringValidationRule()
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var name = (string)value;

            if (string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult(false, "Cannot be empty or whitespace.");
            }

            if (name.Count() < MinLength)
            {
                return new ValidationResult(false, "Must have " + MinLength + " or more characters.");
            }
            if (name.Count() > MaxLength)
            {
                return new ValidationResult(false, "Must have " + MaxLength + " or Less characters.");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class NoWhitespaceRule : ValidationRule
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public NoWhitespaceRule()
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var name = (string)value;

            if (string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult(false, "Cannot be empty or whitespace.");
            }
            if (name.Any(x => char.IsWhiteSpace(x)))
            {
                return new ValidationResult(false, "Cannot contain any whitespace.");
            }
            if (name.Count() < MinLength)
            {
                return new ValidationResult(false, "Must have " + MinLength + " or more characters.");
            }
            if (name.Count() > MaxLength)
            {
                return new ValidationResult(false, "Must have " + MaxLength + " or Less characters.");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class NameStringValidation : ValidationRule
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public Regex valid => new Regex(@"^[a-zA-Z]+$");

        public NameStringValidation()
        {
            MinLength = 0;
            MaxLength = int.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var name = (string)value;

            // Cannot be empty.
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult(false, "Cannot be empty or whitespace.");
            }
            // cannot contain whitespace
            if (name.Any(x => char.IsWhiteSpace(x)))
            {
                return new ValidationResult(false, "Cannot contain any whitespace.");
            }
            // can't contain invalid characters, can only contain
            // a-zA-Z
            if (!valid.IsMatch(name))
            {
                return new ValidationResult(false, "Name can only include a-zA-Z0-9_-.");
            }
            // can't be too short.
            if (name.Count() < MinLength)
            {
                return new ValidationResult(false, "Must have " + MinLength + " or more characters.");
            }
            // can't be too long.
            if (name.Count() > MaxLength)
            {
                return new ValidationResult(false, "Must have " + MaxLength + " or Less characters.");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class IsIntegerRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public IsIntegerRule()
        {
            Min = int.MinValue;
            Max = int.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int val = 0;

            try
            {
                val = int.Parse((string)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal Characters or " + e.Message);
            }

            if (val < Min)
            {
                return new ValidationResult(false,
                    "Value must be Greater than or equal to " + Min);
            }
            if (val > Max)
            {
                return new ValidationResult(false,
                    "Value must be less than or equal to " + Max);
            }

            return new ValidationResult(true, null);
        }
    }

    public class IsDecimalRule : ValidationRule
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }

        public IsDecimalRule()
        {
            Min = decimal.MinValue;
            Max = decimal.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal val = 0;

            try
            {
                val = decimal.Parse((string)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if (val < Min)
            {
                return new ValidationResult(false,
                    "Value must be Greater than or equal to " + Min);
            }
            if (val > Max)
            {
                return new ValidationResult(false,
                    "Value must be less than or equal to " + Max);
            }

            return new ValidationResult(true, null);
        }
    }

    public class Uppercase : ValidationRule
    {
        public Uppercase() {}

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var val = (string)value;

            if (val.Length > 1)
            {
                return new ValidationResult(false,
                    "Must be a singular Uppercase Character.");
            }

            if (!char.IsUpper(val[0]))
            {
                return new ValidationResult(false,
                    "Character Must be Uppercase.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
