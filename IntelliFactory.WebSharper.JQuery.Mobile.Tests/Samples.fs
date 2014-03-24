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

namespace IntelliFactory.WebSharper.JQuery.Mobile.Tests

//namespace Sample

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.JQuery
open IntelliFactory.WebSharper.JQuery.Mobile
open IntelliFactory.WebSharper.Html
//open IntelliFactory.WebSharper.EcmaScript
//open IntelliFactory.WebSharper.JavaScript

type private E =
    IntelliFactory.WebSharper.JQuery.Mobile.Events

[<JavaScript>]
module App =
    // Utilities
    let mobile = Mobile.Instance

    let HeaderDiv cont =
        Div [ HTML5.Attr.Data "role" "header" ] -< cont

    let ContentDiv cont =
        Div [ HTML5.Attr.Data "role" "content" ] -< cont

    let PageDiv id' cont =
        Div [
            HTML5.Attr.Data "role" "page"
            Id id'
        ] -< cont |>! OnAfterRender (fun el ->
            JQuery.Of el.Body |> Mobile.Page.Init
        ) 

    let ListViewUL cont =
        UL [
            HTML5.Attr.Data "role" "listview"
            HTML5.Attr.Data "inset" "true"
        ] -< cont   

    // Data
    type CategoryData =
        {
            Name        : string
            Description : string
            Items       : string list
        }

    let getCategoryData category =
        match category with
        | "animals" -> Some { 
                Name = "Animals"
                Description = "All your favorites from aardvarks to zebras."
                Items = [ "Pets"; "Farm Animals"; "Wild Animals" ] }
        | "colors" -> Some { 
                Name = "Colors"
                Description = "Fresh colors from the magic rainbow."
                Items = [ "Blue"; "Green"; "Orange"; "Purple"; "Red"; "Yellow"; "Violet" ] }
        | "vehicles" -> Some { 
                Name = "Vehicles"
                Description = "Everything from cars to planes."
                Items = [ "Cars"; "Planes"; "Construction" ] }
        | _ -> None

    type JQMPage =
        {
            Html: Element
            Load: unit -> bool
        }

    let changePage (page: string) =
        mobile.ChangePage(page, ChangePageConfig(Transition = "slide"))

    // Page Ids
    module Ids =
        let [<Literal>] HomePage  = "home"
        let [<Literal>] ItemsPage = "items"
    module Refs =
        let [<Literal>] HomePage  = "#home"
        let [<Literal>] ItemsPage = "#items"

    // State
    let mutable selectedCategory = None

    let HomePage =
        let createListItem category text =
            LI [
                A [ HRef ""; Text text ]
                |>! OnClick (fun _ _ ->
                    selectedCategory <- Some category    
                    Refs.ItemsPage |> changePage
                )
            ] 
        {
            Html =
                PageDiv Ids.HomePage [
                    HeaderDiv [ H1 [ Text "Categories" ] ]
                    ContentDiv [
                        H2 [ Text "Select a Category Below:" ]
                        ListViewUL [
                            createListItem "animals"  "Animals"
                            createListItem "colors"   "Colors"
                            createListItem "vehicles" "Vehicles"
                        ]
                    ]
                ] 
            Load = fun() -> true
        }

    let ItemsPage =
        lazy
            let title = H1 []
            let description = P []
            let itemsList = ListViewUL []
            {
                Html =
                    PageDiv Ids.ItemsPage [
                        HeaderDiv [ HTML5.Attr.Data "add-back-btn" "true" ] -< [ title ]
                        ContentDiv [
                            description
                            ListViewUL [ itemsList ]
                        ]
                    ]
                Load = fun() ->
                    match getCategoryData selectedCategory.Value with
                    | Some categoryData ->
                        title.Text <- categoryData.Name
                        description.Text <- categoryData.Description
                        itemsList.Clear()
                        categoryData.Items |> List.iter (fun i -> (LI [Text i]) |> itemsList.Append)
                        JQuery.Of itemsList.Body |> Mobile.ListView.Refresh
                        true
                    | None -> false
            }
        
    let getJQMPage pageRef =
        match pageRef with
        | Refs.HomePage  -> Some HomePage
        | Refs.ItemsPage -> Some ItemsPage.Value
        | _ -> None

type AppControl() =
    inherit Web.Control()

    [<JavaScript>]
    override this.Body =
        Mobile.Events.PageBeforeChange.On(JQuery.Of Dom.Document.Current, fun (e, data) ->
            match data.ToPage with
            | :? string as pageUrl -> 
                match App.getJQMPage pageUrl with
                | Some pageObj ->
                    let body = JQuery.Of "body"                  
                    let toPage =
                        match body.Children pageUrl with
                        | p when p.Length = 0 ->
                            let page = pageObj.Html
                            body.Append page.Body |> ignore
                            (page :> IPagelet).Render()
                            JQuery.Of page.Body
                        | p -> p
                    if not (pageObj.Load()) then e.PreventDefault() //App.mobile.ChangePage(toPage, data.Options)
                | None _ -> ()
            | _ -> ()
        )
        upcast Div [] |>! OnAfterRender (fun _ -> App.Refs.HomePage |> App.mobile.ChangePage)    


module SampleInternals =

    [<JavaScript>]
    let MobileInstance =
        Mobile.Use() // should trigger webresource.
        Mobile.Instance

    [<JavaScript>]
    let SimplePage () =
        let header =
            Div [HTML5.Attr.Data "role" "header"] -< [H1 [Text "Page Title"]]

        let content =
            Div [HTML5.Attr.Data "role" "content" ] -< [
                 P [Text "Lorem ipsum dolor sit amet, consectetur adipiscing"]
            ]

        let footer =
            Div [HTML5.Attr.Data "role" "footer" ] -< [H4 [Text "Page Footer"]]

        let page = 
            Div [HTML5.Attr.Data "role" "page" ] 
            -<  [header; content; footer]

        page
        |>! OnAfterRender (fun page ->
            Mobile.Instance.ChangePage(JQuery.Of(page.Body)))
//        |>! OnAfterRender (fun page ->
//            Mobile.Page.Init(JQuery.Of(page.Body)))


    [<JavaScript>] 
    let HeaderDiv() = Div [HTML5.Attr.Data "role" "header"]

    [<JavaScript>] 
    let ContentDiv() = Div [HTML5.Attr.Data "role" "content"]

    [<JavaScript>] 
    let FooterDiv() = Div [HTML5.Attr.Data "role" "footer"]

    [<JavaScript>] 
    let PageDiv id' = Div [HTML5.Attr.Data "role" "page"; Id id']


    [<JavaScript>]
    let SimpleNavigation () = 
         // should trigger webresource.
        let home =
            let header =
                HeaderDiv() -< [ H1 [ Text "Home" ] ]

            let content =
                ContentDiv() -< [ 
                    P [
                        A [ Attr.HRef ""; Text "About this app" ]               // Attr.HRef "#about"
//                        |>! OnClick (fun _ _ -> MobileInstance.ChangePage(JQuery.Of("#about")) )
                        |>! OnClick (fun _ _ -> MobileInstance.ChangePage(JQuery.Of("#about")) )
                    ] 
                ]  
                
            let footer = FooterDiv() -< [ H1 [Text "Home"] ]             
        
            PageDiv "home" -< [ header; content; footer ]
            |>! OnAfterRender ( fun page ->
                Mobile.Instance.ChangePage(JQuery.Of(page.Body)) )

        let about =
            let header =
                HeaderDiv() -< [ H1 [ Text "About This App" ] ]
        
            let content =
                ContentDiv() -< [
                    P [ Text "This app rocks " ] -< [
                        A [ HRef ""; Text " Go home!" ]
//                        |>! OnClick (fun _ _ -> MobileInstance.ChangePage(JQuery.Of("#home")) )
                        |>! OnClick (fun _ _ -> MobileInstance.ChangePage(JQuery.Of("#home")) )
                    ] 
                ]
                
            let footer = FooterDiv() -< [ H1 [ Text "About This App" ] ]             

            PageDiv "about" -< [ header; content; footer ]
        
        Div [ home; about ]
        
    [<JavaScript>]
    let FormTypes () = 
        let home =
            let header =
                Div [HTML5.Attr.Data "role" "header"
                     H1 [Text "Ice Cream Order Form"] :> IPagelet
                    ]
        
            let content =
                let checkbox (name: string) = 
                    Input [
                        Attr.Type "checkbox" 
                        Name name
                        Id name
                        Attr.Class "custom"
                    ]
                    -- 
                    Label [
                        Attr.For name
                        Text name
                    ] :> IPagelet

                Div [
                    HTML5.Attr.Data "role" "content"
                    Form [ Action "#"
                           Method "get"
                           // Name Field
                           ] -< [
                           Div [
                              HTML5.Attr.Data "role" "fieldcontain"
                              Label [
                                Attr.For "name"
                                Text "Your name:"
                              ]  :> IPagelet
                              Input [
                                Attr.Type "text"
                                Attr.Name "name"
                                Attr.Value ""
                              ]  :> IPagelet
                           ]
                           // Flavour field
                           Div [
                              HTML5.Attr.Data "role" "controlgroup"
                              Legend [
                                Text "Which flavours would you like?"
                              ] :> IPagelet
                              checkbox "Vanilla"
                              checkbox "Chocolate"
                              checkbox "Strawberry"
                           ]
                           // Cones Field
                           Div [
                              HTML5.Attr.Data "role" "fieldcontain"
                              ] -< [
                              Label [
                                Attr.For "quantity"
                                Text "Number of cones:"
                              ]
                              Input [
                                Attr.Type "range"
                                Attr.Name "quantity"
                                Attr.Id   "quantity"
                                Attr.Value "1"
                                HTML5.Attr.Min "1" 
                                HTML5.Attr.Max "100"
                              ]
                           ]
                           // Sprinkles Field
                           Div [
                              HTML5.Attr.Data "role" "fieldcontain" 
                              ] -< [
                              Label [Attr.For "sprinkles";Text "Sprinkles:"]
                              Select [
                                HTML5.Attr.Data "role" "slider"
                                Attr.Name "sprinkles"
                                Attr.Id   "sprinkles"
                                ] -< [
                                Default.Tags.Option [Attr.Value "on"; Text "Yes"]
                                Default.Tags.Option [Attr.Value "off"; Text "No"]
                              ]
                           ]
                           // Store Field
                           Div [
                              HTML5.Attr.Data "role" "fieldcontain" 
                              ] -< [
                              Label [Attr.For "store"; Text "Collect from store:"]
                              Select [
                                Attr.Name "store"
                                Attr.Id   "store"
                                ] -< [
                                Default.Tags.Option [
                                    Attr.Value "mainStreet"
                                    Text "Main Street"
                                ]
                                Default.Tags.Option [
                                    Attr.Value "libertyAvenue"
                                    Text "Liberty Avenue"
                                ]
                                Default.Tags.Option [
                                    Attr.Value "circleSquare"
                                    Text "Circle Square"
                                ]
                                Default.Tags.Option [
                                    Attr.Value "angelRoad"
                                    Text "Angel Road"
                                ]
                              ]
                           ]
                           Div [
                              Attr.Class "ui-body ui-body-b"
                              ] -< [
                              FieldSet [
                                Attr.Class "ui-grid-a"
                                ] -< [
                                Div [ 
                                    Attr.Class "ui-block-a"
                                    Button [
                                        HTML5.Attr.Data "theme" "a"
                                        Attr.Type "submit"
                                        Text "Cancel"
                                    ] :> IPagelet
                                ]
                                Div [
                                    Attr.Class "ui-block-b"
                                    ] -< [
                                    Button [
                                        HTML5.Attr.Data "theme" "a"
                                        Attr.Type "submit"
                                        Text "Order Ice Cream"
                                    ]
                                ]
                              ]
                           ]
                    ] :> IPagelet
                ] :> IPagelet
                
            let page = 
                Div [HTML5.Attr.Data "role" "page"
                     Attr.Id "home"
                     header  :> IPagelet
                     content]
                
            page

        home |>! OnAfterRender (fun el ->
            JQuery.Of el.Body |>! Mobile.Page.Init |> Mobile.Instance.ChangePage
        ) 

    [<JavaScript>]
    let EventTestPage () =
        let header =
            Div [
                HTML5.Attr.Data "role" "header" 
                ] -< [
                H1 [Text "Tap me!"]
            ]

        let jqb = JQuery.JQuery.Of(header.Body)
        E.Tap.On(jqb, fun event -> JavaScript.Alert("Tapped on:" + string event.PageX + "," + string event.PageY))

        let content =
            Div [
                HTML5.Attr.Data "role" "content"
                ] -< [
                P [Text "Swipe me!"]
            ]

        E.Swipe.On(JQuery.Of(content.Body), fun event ->
            JavaScript.Alert("Swipped on"))

        let footer =
            Div [
                HTML5.Attr.Data "role" "footer"
                ] -< [
                H4 [Text "Scroll me!"]
            ]

        E.VMouseDown.On(JQuery.Of(content.Body), fun mouseEvent ->
            JavaScript.Alert(string mouseEvent.Event.PageX))

        E.ScrollStart.On(JQuery.Of(footer.Body), fun event ->
            JavaScript.Alert("Scrolled on"))

        let page = 
            Div [
                HTML5.Attr.Data "role" "page"
                ] -< [
                header
                content
                footer 
            ]

        page    

    [<JavaScript>]
    let UtilsTestPage () =
        let header =
            Div [
                HTML5.Attr.Data "role" "header"
                ] -< [
                H1 [Text "Page 1"]
            ]
        
        let content =
            Div [
                HTML5.Attr.Data "role" "content"
                ] -< [
                P [Text "Content"]
            ]
        
        let page = 
            Div [
                HTML5.Attr.Data "role" "page"
                ] -< [
                header
                content
            ]

        page

type Samples() = 
    inherit Web.Control()

    [<JavaScript>]
    override this.Body = 
//        SampleInternals.SimplePage () :> IPagelet
//        SampleInternals.SimpleNavigation () :> IPagelet
        SampleInternals.FormTypes () :> IPagelet
//        SampleInternals.EventTestPage() :> IPagelet
//        SampleInternals.UtilsTestPage() :> IPagelet

