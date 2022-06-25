using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backres.Models
{
    internal abstract class ActionAbstract
    {

        public ActionAbstract(BackresAction bAction, ActionDirection bDirection)
        {
            if (bAction.ActionName != ActionName)
                throw new Exception("Invalid argument for ActionCopy constructor");
            ItemName = bAction.ItemName;
            Overwrite = bAction.Overwrite;
            ActionDirection = bDirection;
            RegistryKey = bAction.RegistryKey;
            SrcPath = bAction.SrcPath?.NormilizePath(this);
            DstPath = bAction.DstPath?.NormilizePath(this);
        }

        protected abstract string ActionName { get; }

        protected ActionDirection ActionDirection { get; }

        protected bool Overwrite { get; }

        public string ItemName { get; }

        protected string SrcPath { get; }

        protected string DstPath { get; }

        protected string RegistryKey { get; }
    }
}
