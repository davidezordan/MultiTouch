// ****************************************************************************
// <copyright file="IMultiTouchBehavior.cs" company="Davide Zordan">
// Copyright © Davide Zordan 2010-2014
// </copyright>
// ****************************************************************************
// <author>Davide Zordan</author>
// <email>info@davidezordan.net</email>
// <date>01.12.2011</date>
// <project>MultiTouch.Behaviors.Silverlight</project>
// <web>http://multitouch.codeplex.com/</web>
// <license>
// See http://multitouch.codeplex.com/license.
// </license>
// ****************************************************************************
// SAMPLE CODE IS PROVIDED "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE, ARE DISCLAIMED.  IN NO EVENT SHALL ESRI OR CONTRIBUTORS
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) SUSTAINED BY YOU OR A THIRD PARTY, HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT; STRICT LIABILITY; OR TORT ARISING
// IN ANY WAY OUT OF THE USE OF THIS SAMPLE CODE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE TO THE FULL EXTENT ALLOWED BY APPLICABLE LAW.

using System;

namespace MultiTouch.Behaviors.Silverlight.Interfaces
{
    /// <summary>
    /// The interface for the MultiTouchBehavior
    /// </summary>
    public interface IMultiTouchBehavior
    {
        /// <summary>
        /// Gets or sets the value of the <see cref="IsInertiaEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        bool IsInertiaEnabled { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="IsScaleEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        bool IsScaleEnabled { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="IsRotateEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        bool IsRotateEnabled { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="IsTranslateEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        bool IsTranslateEnabled { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="MinimumScale" />
        /// property. This is a dependency property.
        /// </summary>
        double MinimumScale { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="MaximumScale" />
        /// property. This is a dependency property.
        /// </summary>
        double MaximumScale { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="Scale" />
        /// property. This is a dependency property.
        /// </summary>
        double Scale { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="Rotation" />
        /// property. This is a dependency property.
        /// </summary>
        double Rotation { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="CenterX" />
        /// property. This is a dependency property.
        /// </summary>
        double CenterX { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="CenterY" />
        /// property. This is a dependency property.
        /// </summary>
        double CenterY { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="IsPivotEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        bool IsPivotEnabled { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="IgnoreTypes" />
        /// property. This is a dependency property.
        /// </summary>
        Type[] IgnoreTypes { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="IsConstrainedToParentBounds" />
        /// property. This is a dependency property.
        /// </summary>
        bool IsConstrainedToParentBounds { get; set; }

        /// <summary>
        /// Gets or sets the value of the <see cref="AreManipulationsEnabled" />
        /// property. This is a dependency property.
        /// </summary>
        bool AreManipulationsEnabled { get; set; }
    }
}
