using System.Collections.Generic;

namespace HVR.Basis.Vixxy.Runtime
{
    public interface I12VixxyAggregator
    {
        /// Aggregates the data. If the result is different, or if this was never transformed before, this returns true.
        public bool TryAggregate(out IEnumerable<I12VixxyAggregator> aggregators, out IEnumerable<I12VixxyActuator> actuators);
    }
}
