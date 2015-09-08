﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Music.Xml
{
    public class XHelperHasValueResult<T> : IXHelperResult<T>
    {
        private T value;

        public bool HasValue { get; private set; }

        public T Value
        {
            get
            {
                if (!HasValue) throw new Exception("Value does not exist.");
                return value;
            }
        }

        internal XHelperHasValueResult(T value)
        {
            this.value = value;
            HasValue = true;
        }

        internal XHelperHasValueResult()
        {
            HasValue = false;
        }

        public void Then(Action<T> action)
        {
            if (HasValue && action != null) action(Value);
        }

        public void Then(Action action)
        {
            if (HasValue && action != null) action();
        }
    }
}