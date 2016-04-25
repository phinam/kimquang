var $adminCMS = {};
$adminCMS.data = {
    "navigation": {
        "headerNav": [
            {
                "name": "email",
                "url": "#",
                "imgIcon": "",
                "cssIcon": "fa fa-envelope-o",
                "labelType": "success",
                "dropdownClass": "dropdown messages-menu",
                "type": 1,
                "footerMessage": {
                    "text": "See All Messages",
                    "url": "#"
                }
            },
            {
                "name": "notification",
                "url": "#",
                "imgIcon": "",
                "cssIcon": "fa fa-bell-o",
                "labelType": "warning",
                "dropdownClass": "dropdown notifications-menu",
                "type": 2,
                "footerMessage": {
                    "text": "View All",
                    "url": "#"
                }
            },
            {
                "name": "task",
                "url": "#",
                "imgIcon": "",
                "cssIcon": "fa fa-flag-o",
                "labelType": "danger",
                "dropdownClass": "dropdown tasks-menu",
                "type": 3,
                "footerMessage": {
                    "text": "View all tasks",
                    "url": "#"
                }
            },
            {
                "name": "user",
                "url": "#",
                "imgIcon": "Images/icons/user2-160x160.jpg",
                "cssIcon": "",
                "labelType": "",
                "dropdownClass": "dropdown user user-menu",
                "type": 4
            }
        ],

        "sidebarNav": [
            {
                "name": "Manager",
                "url": "#",
                "cssIcon": "fa fa-users text-success",
                "labelCss": "fa fa-angle-left",
                "childs": [
                    { "name": "Account", "url": "account", "cssIcon": "fa fa-flag-o", "childs": "" },
                    { "name": "Employee", "url": "employee", "cssIcon": "fa fa-flag-o", "childs": "" },
                    { "name": "Position", "url": "position", "cssIcon": "fa fa-flag-o", "childs": "" },
                    { "name": "Department", "url": "department", "cssIcon": "fa fa-flag-o", "childs": "" },
                    { "name": "Area", "url": "area", "cssIcon": "fa fa-flag-o", "childs": "" },
                    { "name": "Supplier", "url": "supplier", "cssIcon": "fa fa-flag-o", "childs": "" }
                    
                ]
            },
            {
                "name": "Project Management",
                "url": "#",
                "cssIcon": "fa  fa-street-view",
                "labelCss": "fa fa-angle-left",
                "childs": [
                    { "name": "Project", "url": "project", "cssIcon": "fa fa-flag-o", "childs": "" },
                ]
            },
        ]
    },

    "user": {
        "userName": "mrThanh",
        "email": [
            { "title": "Support Team 1", "image": "Images/icons/user2-160x160.jpg", "cssIcon": "fa fa-clock-o", "receivedAt": "Fri May 14 2015 01:19:48 GMT+0700 (SE Asia Standard Time)", "message": "Why not buy a new awesome theme?", "url": "#" },
            { "title": "Support Team 2", "image": "Images/icons/user3-128x128.jpg", "cssIcon": "fa fa-clock-o", "receivedAt": "Fri May 12 2015 02:19:48 GMT+0700 (SE Asia Standard Time)", "message": "Why not buy a new awesome theme?", "url": "#" },
            { "title": "Support Team 3", "image": "Images/icons/user4-128x128.jpg", "cssIcon": "fa fa-clock-o", "receivedAt": "Fri May 15 2015 07:15:48 GMT+0700 (SE Asia Standard Time)", "message": "Why not buy a new awesome theme?", "url": "#" },
            { "title": "Support Team 4", "image": "Images/icons/user2-160x160.jpg", "cssIcon": "fa fa-clock-o", "receivedAt": "Fri May 15 2015 05:19:48 GMT+0700 (SE Asia Standard Time)", "message": "Why not buy a new awesome theme?", "url": "#" },
            { "title": "Support Team 5", "image": "Images/icons/user2-160x160.jpg", "cssIcon": "fa fa-clock-o", "receivedAt": "Fri May 15 2015 03:19:48 GMT+0700 (SE Asia Standard Time)", "message": "Why not buy a new awesome theme?", "url": "#" },
            { "title": "Support Team 6", "image": "Images/icons/user2-160x160.jpg", "cssIcon": "fa fa-clock-o", "receivedAt": "Fri May 15 2015 07:00:48 GMT+0700 (SE Asia Standard Time)", "message": "Why not buy a new awesome theme?", "url": "#" }
        ],
        "notification": [
            { "cssIcon": "fa fa-users text-aqua", "message": "5 new members joined today", "url": "#" },
            { "cssIcon": "fa fa-warning text-yellow", "message": "Very long description here that may not fit into the page and may cause design problems", "url": "#" },
            { "cssIcon": "fa fa-users text-red", "message": "5 new members joined", "url": "#" },
            { "cssIcon": "fa fa-shopping-cart text-green", "message": "25 sales made", "url": "#" },
            { "cssIcon": "fa fa-user text-red", "message": "You changed your username", "url": "#" }
        ],
        "task": [
            { "title": "Design some buttons", "completePercent": 20, "url": "#" },
            { "title": "Create a nice theme", "completePercent": 40, "url": "#" },
            { "title": "Some task I need to do", "completePercent": 60, "url": "#" },
            { "title": "Make beautiful transitions", "completePercent": 80, "url": "#" }
        ],
        "profile": {
            "fullName": "Thanh Ly",
            "title": "Thanh Ly - Web Developer",
            "image": "Images/icons/user2-160x160.jpg",
            "beMemberSince": "Nov 2015",
            "profileUrl": "#",
            "userBody": [
                { "text": "Followers", "url": "#" },
                { "text": "Sales", "url": "#" },
                { "text": "Friends", "url": "#" }
            ]
        }
    },
    "serverList": [
        { "id": 1, "name": "Server 1" },
        { "id": 2, "name": "Server 2" },
        { "id": 3, "name": "Server 3" }
    ],
    "server":
        {
            "id": 1,
            "name": "Server 1",
            "folderTree": [
            {
                "name": "C:",
                "url": "#",
                "cssIcon": "fa fa-laptop",
                "type": "driver",
                "server": 1,
                "labelCss": "fa fa-angle-left",
                "childs": [
                    { "name": "jquery.js", "url": "uploadFiles", "cssIcon": "fa fa-flag-o", "type": "js", "childs": "" },
                    { "name": "jquery.min.js", "url": "listFiles", "cssIcon": "fa fa-flag-o", "type": "js", "childs": "" }
                ]
            },
            {
                "name": "D:",
                "url": "#",
                "cssIcon": "fa fa-laptop",
                "labelCss": "fa fa-angle-left",
                "type": "driver",
                "server": 1,
                "childs": [
                    { "name": "toaster.js", "url": "db1", "cssIcon": "fa fa-flag-o", "type": "js", "childs": "" },
                ]
            },
            {
                "name": "E:",
                "url": "#",
                "cssIcon": "fa fa-th",
                "labelCss": "label bg-green",
                "type": "driver",
                "server": 1,
                "childs": ""
            },
            {
                "name": "F:",
                "url": "#",
                "cssIcon": "fa fa-share",
                "labelCss": "fa fa-angle-left",
                "type": "driver",
                "server": 1,
                "childs": [
                    {
                        "name": "angularRoute",
                        "url": "#",
                        "cssIcon": "fa fa-flag-o",
                        "childs": [
                            {
                                "name": "Level Two",
                                "url": "#",
                                "cssIcon": "fa fa-flag-o",
                                "childs": [
                                    { "name": "angular.route.js", "url": "#", "cssIcon": "fa fa-flag-o", "type": "js", "childs": "" },
                                    { "name": "angular.route.min.js", "url": "#", "cssIcon": "fa fa-flag-o", "type": "js", "childs": "" }
                                ]
                            }
                        ]
                    },
                    { "name": "angular.js", "url": "#", "cssIcon": "fa fa-flag-o", "childs": "" },
                    { "name": "angular.min.js", "url": "#", "cssIcon": "fa fa-flag-o", "childs": "" }
                ]
            }
            ]
        }
};
var layoutConfig={
    skin: ' skin-green-light'
}