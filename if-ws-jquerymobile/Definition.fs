// $begin{copyright}
//
// This file is confidential and proprietary.
//
// Copyright (c) IntelliFactory, 2004-2010.
//
// All rights reserved.  Reproduction or use in whole or in part is
// prohibited without the written consent of the copyright holder.
//-----------------------------------------------------------------
// $end{copyright}

namespace IntelliFactory.WebSharper.JQuery.Mobile

module Definition =
    open IntelliFactory.WebSharper.InterfaceGenerator
    open IntelliFactory.WebSharper.Dom
    open IntelliFactory.WebSharper

    let EventHandler =
        (
            T<Element>  -*
            T<JQuery.Event>?event
        ) ^-> T<unit>        

    let JQ = Type.New()

    let Mobile =
        Class "Mobile"
        |+> Protocol [
            "subPageUrlKey" =@ T<string>
            |> WithComment "The url parameter used for referencing widget-generated sub-pages (such as those generated by nested listviews). Translates to to example.html&ui-page=subpageIdentifier. The hash segment before &ui-page= is used by the framework for making an Ajax request to the URL where the sub-page exists."
            "nonHistorySelectors" =@ T<string>
            |> WithComment "Anchor links with a data-rel attribute value, or pages with a data-role value, that match these selectors will not be trackable in history (they won't update the location.hash and won't be bookmarkable)."
            "activePageClass" =@ T<string>
            |> WithComment "The class assigned to page currently in view, and during transitions"
            "activeBtnClass" =@ T<string>
            |> WithComment "The class used for \"active\" button state, from CSS framework."
            "ajaxLinksEnabled" =@ T<bool>
            |> WithComment "jQuery Mobile will automatically handle link clicks through Ajax, when possible."
            "ajaxFormsEnabled" =@ T<bool>
            |> WithComment "jQuery Mobile will automatically handle form submissions through Ajax, when possible."
            "transitions" =@ T<string[]>
            |> WithComment "Available CSS transitions in the CSS."
            "defaultTransition" =@ T<string>
            |> WithComment "Set the default transition for page changes that use Ajax. Set to 'none' for no transitions by default."
            "loadingMessage" =@ T<string>
            |> WithComment "Set the text that appears when a page is loading. If set to false, the message will not appear at all."
            "metaViewportContent" =@ T<string>
            |> WithComment "Configure the auto-generated meta viewport tag's content attribute. If false, no meta tag will be appended to the DOM."
            "gradeA" =@ T<unit -> bool>
            |> WithComment "Any support conditions that must be met in order to proceed."

            "changePage" => ((T<JQuery.JQuery> + JQ + T<string> + T<string[]> + T<obj>) 
                            * !? T<string>?to_
                            * !? T<bool>?transition
                            * !? T<bool>?back)
                            ^-> T<unit>
            |> WithComment "Programmatically change from one page to another. This method is used internally for transitions that occur as a result of clicking a link or submitting a form, when those features are enabled."


            "pageLoading" => !?T<bool>?done_ ^-> T<unit>
            |> WithComment "Show or hide the page loading message, which is configurable via $.mobile.loadingMessage."

            "silentScroll" => !?T<int>?yPos ^-> T<unit>
            |> WithComment "Scroll to a particular Y position without triggering scroll event listeners."

            "addResolutionBreakpoints" => (T<int> + T<int[]>)?values ^-> T<unit>
            |> WithComment "Add width breakpoints to the min/max width classes that are added to the HTML element."

            "activePage" =? JQ
            |> WithComment "Reference to the page currently in view."

        ]

    let JQuery = 
        Class "jQuery"
        |=> JQ
        |+> Protocol [
            "base" =? T<IntelliFactory.WebSharper.JQuery.JQuery> |> WithGetterInline "$this"
        
            // Tap
            "tap" => !?EventHandler?handler ^-> JQ
            |> WithComment "Triggers after a quick, complete touch event."

            // taphold
            "taphold" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggers after a held complete touch event (close to one second)."

            // swipe
            "swipe" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggers when a horizontal drag of 30px or more (and less than 20px vertically) occurs within 1 second duration."

            // swipeleft
            "swipeleft" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggers when a swipe event occurred moving in the left direction."

            // swiperight
            "swiperight" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggers when a swipe event occurred moving in the right direction."

            // orientationchange
            "orientationchange" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggers when a device orientation changes (by turning it vertically or horizontally). When bound to this event, your callback function can leverage a second argument, which contains an orientation property equal to either \"portrait\" or \"landscape\". These values are also added as classes to the HTML element, allowing you to leverage them in your CSS selectors. Note that we currently bind to the resize event when orientationChange is not natively supported."

            // scrollstart
            "scrollstart" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggers when a scroll begins. Note that iOS devices freeze DOM manipulation during scroll, queuing them to apply when the scroll finishes. We're currently investigating ways to allow DOM manipulations to apply before a scroll starts."

            // scrollstop
            "scrollstop" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggers when a scroll finishes."

            // pagebeforeshow
            "pagebeforeshow" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggered on the page being shown, before its transition begins."

            // pagebeforehide
            "pagebeforehide" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggered on the page being hidden, before its transition begins."

            // pageshow
            "pageshow" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggered on the page being shown, after its transition completes."

            // pagehide
            "pagehide" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggered on the page being hidden, after its transition completes."

            // pagebeforecreate
            "pagebeforecreate" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggered on the page being initialized, before initialization occurs."

            // pagecreate
            "pagecreate" => !? EventHandler?handler ^-> JQ
            |> WithComment "Triggered on the page being initialized, after initialization occurs."

            // animationComplete
            "animationComplete" => !? EventHandler?handler ^-> JQ
            |> WithComment "jQuery Mobile exposes the animationComplete plugin, which you can utilize after adding or removing a class that applies a CSS transition."
        ] 
        |+> [   
            "of" => T<JQuery.JQuery>?jquery ^-> JQ
            |> WithInline "$jquery"
            |> WithComment "Builds a jquery mobile extended object with a normal jquery object."
            "UseJQueryMobile" =? T<unit>
            |> WithGetterInline "undefined"
            |> WithComment "Just a NOP to force the usage of JQuery mobile."
            "mobile" =? Mobile
            |> WithGetterInline "jQuery.mobile"
            |> WithComment "Mobile configurable options."
        ]

    let ButtonIcon = 
        Pattern.EnumInlines "ButtonIcon" [
            "LeftArrow", "'arrow-l'"
            "RightArrow", "'arrow-r'"
            "UpArrow", "'arrow-u'"
            "DownArrow", "'arrow-d'"
            "Delete", "'delete'"
            "Plus", "'plus'"
            "Minus", "'minus'"
            "Check", "'check'"
            "Gear", "'gear'"
            "Refresh", "'refresh'"
            "Forward", "'forward'"
            "Back", "'back'"
            "Grid", "'grid'"
            "Star", "'star'"
            "Alert", "'alert'"
            "Info", "'info'"
        ]
    
    let Theme = 
        Pattern.EnumStrings "Theme" [
            "a"
            "b"
            "c"
            "d"
            "e"
        ]
    
    let Assembly =
        Assembly [
            Namespace "IntelliFactory.WebSharper.JQuery.Mobile" [
                JQuery
                Mobile
            ]
            Namespace "IntelliFactory.WebSharper.JQuery.Mobile.Enums" [
                ButtonIcon
                Theme
            ]
        ]
