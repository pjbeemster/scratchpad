$.validator.unobtrusive.adapters.add("mycustomvalidation", ['otherproperty0', 'otherproperty1'],
    function (options) {
        options.rules['mycustomvalidation'] = { other: options.params.other,
            otherproperty0: options.params.otherproperty0,
            otherproperty1: options.params.otherproperty1
        };
        options.messages['mycustomvalidation'] = options.message;
    }
);


$.validator.addMethod("mycustomvalidation", function (value, element, params) {
    var retVal = false;
    if ($(element)) {
        retVal = $(element).attr("checked");
    }
    if (retVal == true) {
        return retVal;
    }

    if (params.otherproperty0) {
        if ($('#' + params.otherproperty0)) {
            retVal = $('#' + params.otherproperty0).attr("checked");
        }
    }
    if (retVal == true) {
        return retVal;
    }

    if (params.otherproperty1) {
        if ($('#' + params.otherproperty1)) {
            retVal = $('#' + params.otherproperty1).attr("checked");
        }
    }
    if (retVal == true) {
        return retVal;
    }


    return false;
});


$.validator.unobtrusive.adapters.add("customrequiredvalidation", ['otherproperty0', 'otherproperty1'],
    function (options) {
        options.rules['customrequiredvalidation'] = { other: options.params.other,
            otherproperty0: options.params.otherproperty0,
            otherproperty1: options.params.otherproperty1
        };
        options.messages['customrequiredvalidation'] = options.message;
    }
);

$.validator.addMethod("customrequiredvalidation", function (value, element, params) {
    var retVal = false;

    if (value) {
        return true;
    }

    return false;

});


$.validator.unobtrusive.adapters.add("customrequiredifvalidation", ['otherproperty0', 'otherproperty1'],
    function (options) {
        options.rules['customrequiredifvalidation'] = { other: options.params.other,
            otherproperty0: options.params.otherproperty0,
            otherproperty1: options.params.otherproperty1
        };
        options.messages['customrequiredvalidation'] = options.message;
    }
);

    $.validator.addMethod("customrequiredifvalidation", function (value, element, params) {
        var retVal = false;

        if (value) {
            alert("true");
            return true;
        } else {
            alert("false");
        }

        return false;

    });