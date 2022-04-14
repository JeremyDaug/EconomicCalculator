﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Editor.ValidationRules
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

    public class AlphaNumStringValidation : ValidationRule
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public Regex valid => new Regex(@"^[a-zA-Z ]+$");

        public AlphaNumStringValidation()
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

    public class NameStringValidation : ValidationRule
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public Regex valid => new Regex(@"^[a-zA-Z0-9 ]+$");

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
            // can't contain invalid characters, can only contain
            // a-zA-Z0-9 and spaces
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

    public class IsULongRule : ValidationRule
    {
        public ulong Min { get; set; }
        public ulong Max { get; set; }

        public IsULongRule()
        {
            Min = 0;
            Max = ulong.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ulong val = 0;

            try
            {
                val = ulong.Parse((string)value);
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
            var strForm = ((string)value);

            if (strForm.Count() > 1)
            {
                return new ValidationResult(false,
                    "Can only be 1 character long.");
            }

            var val = strForm[0];

            if (!char.IsUpper(val))
            {
                return new ValidationResult(false,
                    "Character Must be Uppercase.");
            }

            return new ValidationResult(true, null);
        }
    }
}
