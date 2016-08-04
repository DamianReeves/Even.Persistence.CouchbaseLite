using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Even.Persistence.CouchbaseLite.Messages
{
    [Equals]
    public class SequenceIncremented
    {
        public SequenceIncremented(long counter)
        {
            Counter = counter;
        }

        public long Counter { get; }
    }

    [Equals]
    public class SequenceNumbersReserved
    {

        public SequenceNumbersReserved(long start, long end)
        {
            Start = start;
            End = end;
        }

        public long Start { get; }
        public long End { get; }
    }

    [Equals]
    public class SequenceNumberReservationRejected
    {
        public SequenceNumberReservationRejected(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }

    [Equals]
    internal class SequenceStarted
    {
        public SequenceStarted(long next)
        {
            Next = next;
        }

        public long Next { get; }
    }

    [Equals]
    public class OperationCompleted
    {
        public OperationCompleted(Type operationType, bool completedSuccessfully, Exception error)
        {
            OperationType = operationType;
            CompletedSuccessfully = completedSuccessfully;
            Error = error;
        }

        public Type OperationType { get; }
        public bool CompletedSuccessfully { get; }
        public Exception Error { get; }

        public static OperationCompleted Successfully(Type operationType)
        {
            return new OperationCompleted(operationType, true, null);
        }

        public static OperationCompleted WithFailure(Type operationType)
        {
            return new OperationCompleted(operationType, false, null);
        }

        public static OperationCompleted WithError(Type operationType, Exception error)
        {
            return new OperationCompleted(operationType, false, error);
        }

        public static OperationCompleted WithErrors(Type operationType, IEnumerable<Exception> errors)
        {
            return WithErrors(operationType, errors?.ToArray() ?? new Exception[] {});
        }
        public static OperationCompleted WithErrors(Type operationType, params Exception[] errors)
        {
            if (errors == null || errors.Length == 0)
            {
                return new OperationCompleted(operationType, false, null);
            }

            if (errors.Length == 1)
            {
                return new OperationCompleted(operationType, false, errors.First());
            }

            var aggregate = new AggregateException(errors);
            return new OperationCompleted(operationType, false, aggregate);
        }
    }
}
