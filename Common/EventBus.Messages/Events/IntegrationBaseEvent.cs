using System;

namespace EventBus.Messages.Events {
    public class IntegrationBaseEvent {
        public IntegrationBaseEvent () {
            Id = Guid.NewGuid ();
            CreationDate = DateTime.UtcNow;
            Metadata = new Dictionary<string, string> ();
        }

        public IntegrationBaseEvent (Guid id, DateTime createDate) {
            Id = id;
            CreationDate = createDate;
            Metadata = new Dictionary<string, string> ();
        }

        public Guid Id { get; private set; }

        public DateTime CreationDate { get; private set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}