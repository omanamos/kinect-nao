using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    class NaoHand
    {
        private bool opened;

        private NaoHand() { }

        public NaoHand(bool opened)
        {
            this.opened = opened;
        }

        public bool isOpen()
        {
            return this.opened;
        }
    }
}
