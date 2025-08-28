
using System;

namespace Contracts
{
    public class StartRunCommand
    {
        public string DatasetPath { get; set; }
        public Guid RunId { get; set; }
    }
}
