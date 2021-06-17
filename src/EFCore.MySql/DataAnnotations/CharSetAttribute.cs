﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.DataAnnotations
{
    /// <summary>
    ///     Configures the property as capable of persisting unicode characters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class CharSetAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CharSetAttribute" /> class.
        ///     Implicitly uses <see cref="Microsoft.EntityFrameworkCore.DelegationModes.ApplyToAll"/>.
        /// </summary>
        /// <param name="charSet"> The name of the character set to use. </param>
        public CharSetAttribute(string charSet)
            : this(charSet, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CharSetAttribute" /> class.
        /// </summary>
        /// <param name="charSet"> The name of the character set to use. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not.
        /// Ignored when <see cref="CharSetAttribute"/> is applied to properties/columns.
        /// </param>
        public CharSetAttribute(string charSet, DelegationModes delegationModes)
            : this(charSet, (DelegationModes?)delegationModes)
        {
        }

        protected CharSetAttribute(string charSet, DelegationModes? delegationModes)
        {
            CharSetName = charSet;
            DelegationModes = delegationModes;
        }

        /// <summary>
        ///     The name of the character set to use.
        /// </summary>
        public virtual string CharSetName { get; }

        /// <summary>
        /// Finely controls where to recursively apply the character set and where not.
        /// Implicitly uses <see cref="Microsoft.EntityFrameworkCore.DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// Ignored when <see cref="CharSetAttribute"/> is applied to properties/columns.
        /// </summary>
        public virtual DelegationModes? DelegationModes { get; }
    }
}
