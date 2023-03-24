using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Models
{
    public abstract class Command
    {
        public virtual string name => GetType().Name;
        public bool executed { get; private set; }

        protected abstract Result OnExecute();
		protected abstract Result OnRevert();

		public Result Execute()
        {
            if (executed)
            {
                return false;
            }

            return OnExecute();
        }

        public Result Revert()
        {
            if (!executed)
            {
                return false;
            }

            return OnRevert();
        }
    }
}
