// $(selector).dragonFacebook({
//   appId:     "facebook-app-id", 
//   loginUrl:  "/context/LoginFacebook", 
//   logoutUrl: "/context/Logout"
// });

; (function ($) {

    var NAMESPACE = "dragon.context.facebook";

    function namespaced(s) {
        return s + "." + NAMESPACE;
    }

    var methods = {

        init: function (options) {

            return this.each(function () {

                var settings = {
                    appId: "",
                    loginUrl: "/facebook/login",
                    logoutUrl: "/facebook/logout"
                };

                if (options) {
                    $.extend(settings, options);
                }

                var plugin = this;
                var $plugin = $(this);

                $plugin.settings = settings;

                if ($plugin.settings.appId.length == 0) alert("Facebook AppId not set.");

                this.loadJSFromFacebook = function() {
                    // load javascript from facebook
                    (function(d) {
                        var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
                        if (d.getElementById(id)) {
                            return;
                        }
                        js = d.createElement('script');
                        js.id = id;
                        js.async = true;
                        js.src = "//connect.facebook.net/en_US/all.js";
                        ref.parentNode.insertBefore(js, ref);
                    }(document));
                };

                this.login = function(credentials) {
                    FB.login(this.processAuthResult);
                }
                
                this.processAuthResult = function(response) {
                    if (response.authResponse) {
                        var credentials = {
                            key: response.authResponse.userID,
                            secret: response.authResponse.accessToken
                        };
                        plugin.postLogin(credentials);
                    } else {
                        $plugin.trigger(namespaced("declined"));
                    }
                }

                this.postLogin = function(credentials) {
                    $.ajax({
                        url: $plugin.settings.loginUrl,
                        type: "POST",
                        data: credentials,
                        error: function(xhr, ajaxOptions, thrownError) {
                            $plugin.trigger(namespaced("loginError"), thrownError);
                            $plugin.trigger(namespaced("error"), thrownError);
                        },
                        success: function(result) {
                            var arr = result.split("#");
                            var command = arr[0];
                            var arg = "";
                            if (arr.length > 1) {
                                arg = arr[1];
                            }
                            switch(command) {
                                case "redirect":
                                    window.location = arg;
                                    break;
                                case "alert":
                                    alert(arg);
                                    break;
                            }
                            $plugin.trigger(namespaced("loginSuccessful"));
                        }
                    });
                };

                this.postLogout = function() {
                    $.ajax({
                        url: $plugin.settings.logoutUrl,
                        type: "POST",
                        data: {},
                        error: function(xhr, ajaxOptions, thrownError) {
                            $plugin.trigger(namespaced("logoutError"), thrownError);
                            $plugin.trigger(namespaced("error"), thrownError);
                        },
                        success: function() {
                            $plugin.trigger(namespaced("logoutSuccessful"));
                        }
                    });
                };

                // 
                $plugin.data(NAMESPACE, {});

                this.loadJSFromFacebook();

                window.fbAsyncInit = function () {
                    FB.init({
                        appId: $plugin.settings.appId,
                        status: true,
                        cookie: true,
                        xfbml: true
                    });

                    FB.Event.subscribe('auth.login', this.processAuthResult);

                    FB.Event.subscribe('auth.logout', function (response) {
                        plugin.postLogout();
                    });
                    
                    FB.Event.subscribe('auth.statusChange', function (response) {
                       
                    });
                    
                    FB.Event.subscribe('auth.authResponseChange', function (response) {
                        
                    });

                    $plugin.trigger("loaded");
                };
            });

        }, // init

        
        login: function () {
            return this.each(function() {
                this.login();
            });
        },

        logout: function () {
            return this.each(function() {
                this.logout();
            });
        }
    };

    $.fn.dragonFacebook = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        }
        else {
            $.error("Method " + method + " does not exist on this object.");
        }
    };

})(jQuery);