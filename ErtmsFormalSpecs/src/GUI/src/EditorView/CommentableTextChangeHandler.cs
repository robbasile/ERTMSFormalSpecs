// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using DataDictionary;

namespace GUI.EditorView
{
    /// <summary>
    ///     Sets the string value into the right property
    /// </summary>
    public class CommentableTextChangeHandler : Window.HandleTextChange
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance"></param>
        public CommentableTextChangeHandler(ModelElement instance)
            : base(instance, "Comment")
        {
        }

        /// <summary>
        ///     The way text is retrieved from the instance
        /// </summary>
        /// <returns></returns>
        public override string GetText()
        {
            string retVal = "";
            ICommentable commentable = Instance as ICommentable;

            if (commentable != null)
            {
                retVal = commentable.Comment;
            }
            return retVal;
        }

        /// <summary>
        ///     The way text is set back in the instance
        /// </summary>
        /// <returns></returns>
        public override void SetText(string text)
        {
            text = RemoveUselessCharacters(text);

            ICommentable commentable = Instance as ICommentable;
            if (commentable != null)
            {
                // We don't care about changes in only \r
                string originalText = RemoveUselessCharacters(commentable.Comment);

                if (originalText != text)
                {
                    commentable.Comment = text;
                }
            }
        }
    }
}