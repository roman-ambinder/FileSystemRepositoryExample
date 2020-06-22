using System;

namespace Roman.Ambinder.Infra.Common.DataTypes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance",
       "U2U1004:Public value types should implement equality",
       Justification = "<Pending>")]
    public readonly struct OperationResult
    {
        public static readonly OperationResult Successful =
            new OperationResult(success: true);

        public OperationResult(Exception ex)
            : this(success: false, errorMessage: ex.Message)
        { }

        public OperationResult(bool success, string errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static implicit operator bool(OperationResult opRes)
            => opRes.Success;

        public override string ToString()
            => $"{nameof(Success)}: {Success}, {nameof(ErrorMessage)}: {ErrorMessage}";

        public bool Success { get; }

        public string ErrorMessage { get; }
    }
}