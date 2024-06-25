// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace Reelity.Core.Portal.Web.Views.Bases
{
    public partial class TextBoxBase
    {
        [Parameter]
        public string Value { get; set; }
        [Parameter]
        public string PlaceHolder { get; set; }

        public void SetValue(string value) => 
            this.Value = value;
    }
}
