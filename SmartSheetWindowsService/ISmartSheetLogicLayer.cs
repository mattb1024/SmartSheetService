using System;

namespace SmartSheetWindowsService
{
    /// <summary>
    /// This provides interfaces to the SampleLogicLayer class.
    /// </summary>
    internal interface ISmartSheetLogicLayer : IDisposable
    {
        /// <summary>
        /// Runs the business logic here.
        /// </summary>
        void Run();
    }
}