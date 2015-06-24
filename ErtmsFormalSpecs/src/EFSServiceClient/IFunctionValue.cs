namespace EFSServiceClient
{
    /// <summary>
    /// A functional value
    /// </summary>
    interface IFunctionValue
    {
        /// <summary>
        /// Evaluates the function for a given value for X
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        double Evaluate(double x);
    }
}
