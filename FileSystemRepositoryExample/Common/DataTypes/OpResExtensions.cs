using System;
using System.Runtime.CompilerServices;

namespace Roman.Ambinder.Infra.Common.DataTypes
{
    /// <summary>
    ///
    /// </summary>
    public static class OpResExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OperationResultOf<T> AsSuccessfullOpRes<T>(this T target)
            => new OperationResultOf<T>(target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OperationResultOf<TBase> AsSuccessfullOpRes<TImpl, TBase>(
            this TImpl target)
            where TImpl : TBase
          => new OperationResultOf<TBase>((TBase)target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OperationResultOf<T> AsFailedOpRes<T>(this Exception target)
           => new OperationResultOf<T>(target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OperationResultOf<T> AsFailedOpResOf<T>(this OperationResult opRes)
         => new OperationResultOf<T>(false, default, opRes.ErrorMessage);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OperationResultOf<T> AsFailedOpResOf<T>(this string errorMessage)
          => new OperationResultOf<T>(success: false, value: default, errorMessage: errorMessage);
    }
}