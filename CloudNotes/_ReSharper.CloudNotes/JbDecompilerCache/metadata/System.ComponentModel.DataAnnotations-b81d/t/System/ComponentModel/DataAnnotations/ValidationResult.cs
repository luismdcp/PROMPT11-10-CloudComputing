// Type: System.ComponentModel.DataAnnotations.ValidationResult
// Assembly: System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.ComponentModel.DataAnnotations.dll

using System.Collections.Generic;
using System.Runtime;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Represents a container for the results of a validation request.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// true if validation was successful; otherwise, false.
        /// </summary>
        public static readonly ValidationResult Success;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class by using an error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public ValidationResult(string errorMessage);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class by using an error message and a list of members that have validation errors.
        /// </summary>
        /// <param name="errorMessage">The error message.</param><param name="memberNames">The list of member names that have validation errors.</param>
        public ValidationResult(string errorMessage, IEnumerable<string> memberNames);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class by using a <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> object.
        /// </summary>
        /// <param name="validationResult">The validation result object.</param>
        protected ValidationResult(ValidationResult validationResult);

        /// <summary>
        /// Gets the collection of member names that indicate which fields have validation errors.
        /// </summary>
        /// 
        /// <returns>
        /// The collection of member names that indicate which fields have validation errors.
        /// </returns>
        public IEnumerable<string> MemberNames { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; }

        /// <summary>
        /// Gets the error message for the validation.
        /// </summary>
        /// 
        /// <returns>
        /// The error message for the validation.
        /// </returns>
        public string ErrorMessage { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        set; }
    }
}
