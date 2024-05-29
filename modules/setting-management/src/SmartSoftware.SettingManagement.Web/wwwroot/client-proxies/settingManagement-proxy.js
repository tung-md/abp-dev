/* This file is automatically generated by SS framework to use MVC Controllers from javascript. */


// module settingManagement

(function(){

  // controller smartsoftware.settingManagement.emailSettings

  (function(){

    ss.utils.createNamespace(window, 'smartsoftware.settingManagement.emailSettings');

    smartsoftware.settingManagement.emailSettings.get = function(ajaxParams) {
      return ss.ajax($.extend(true, {
        url: ss.appPath + 'api/setting-management/emailing',
        type: 'GET'
      }, ajaxParams));
    };

    smartsoftware.settingManagement.emailSettings.update = function(input, ajaxParams) {
      return ss.ajax($.extend(true, {
        url: ss.appPath + 'api/setting-management/emailing',
        type: 'POST',
        dataType: null,
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    smartsoftware.settingManagement.emailSettings.sendTestEmail = function(input, ajaxParams) {
      return ss.ajax($.extend(true, {
        url: ss.appPath + 'api/setting-management/emailing/send-test-email',
        type: 'POST',
        dataType: null,
        data: JSON.stringify(input)
      }, ajaxParams));
    };

  })();

  // controller smartsoftware.settingManagement.timeZoneSettings

  (function(){

    ss.utils.createNamespace(window, 'smartsoftware.settingManagement.timeZoneSettings');

    smartsoftware.settingManagement.timeZoneSettings.get = function(ajaxParams) {
      return ss.ajax($.extend(true, {
        url: ss.appPath + 'api/setting-management/timezone',
        type: 'GET'
      }, { dataType: 'text' }, ajaxParams));
    };

    smartsoftware.settingManagement.timeZoneSettings.getTimezones = function(ajaxParams) {
      return ss.ajax($.extend(true, {
        url: ss.appPath + 'api/setting-management/timezone/timezones',
        type: 'GET'
      }, ajaxParams));
    };

    smartsoftware.settingManagement.timeZoneSettings.update = function(timezone, ajaxParams) {
      return ss.ajax($.extend(true, {
        url: ss.appPath + 'api/setting-management/timezone' + ss.utils.buildQueryString([{ name: 'timezone', value: timezone }]) + '',
        type: 'POST',
        dataType: null
      }, ajaxParams));
    };

  })();

})();

