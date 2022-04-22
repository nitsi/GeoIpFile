using System;

namespace Kubernetes1.Throttling
{
    public class ThrottlingItem
    {
        public volatile int Count;

        public DateTime WindowDateTime { get; set; }
    }
}