$.ajax({
    type: "GET",
    traditional: true,
    async: true,
    cache: false,
    url: '/Statistici',
    success: function (result) {
       // alert(result);
    },
    error: function (xhr) {
        //debugger;  
        console.log(xhr.responseText);
        alert("Error has occurred..");
    }
});  


//$(document).ready(function () {
    // This WILL work because we are listening on the 'document', 
    // for a click on an element with an ID of #test-element
    $(document).on("click", ".btnModal", function () {
       
        var url = $(this).attr('data-url');
        $('.modal-content').load(url);

        $('#MModal').modal('show');
    });


$(document).on("click", ".cr", function () {

    var check = $(this).parent().children('input');
    var partialUrl = $(this).attr("data-url");
    var urlP = "";
    //alert(check.is(":checked"));
    if (check.is(":checked")) {
        urlP = partialUrl + "&stare=1";
    }
    else {
        urlP = partialUrl + "&stare=2";
    }
        // it is checked

        $.ajax({
            type: "GET",
            traditional: true,
            async: true,
            cache: false,
            url: urlP,
            success: function (result) {
                $.notify(result.mesaj, "success");
            },
            error: function (result) {
                $.notify(result.mesaj, "error");
            }
        });
    

});