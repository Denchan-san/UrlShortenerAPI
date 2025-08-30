﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.Exceptions
{
    public class UrlAlreadyExistsException : Exception
    {
        public UrlAlreadyExistsException(string message) : base(message) { }
    }

}
