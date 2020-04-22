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

    // This will NOT work because there is no '#test-element' ... yet
    $("#test-element").on("click", function () {
        alert("click bound directly to #test-element");
    });

    // Create the dynamic element '#test-element'
    $('body').append('<div id="test-element">Click mee</div>');
//});
    