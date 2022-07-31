function changeLogo() {
	$('#logo').click();
}
$('#logo').change(function () {
	var imgPath = this.value;
	var ext = imgPath.substring(imgPath.lastIndexOf('.') + 1).toLowerCase();
	if (ext == "gif" || ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "gif")
		readURL(this);
	else
		alert("Por favor seleccione una imagen (jpg, jpeg, png, gif).")
});
function readURL(input) {
	if (input.files && input.files[0]) {
		var reader = new FileReader();
		reader.readAsDataURL(input.files[0]);
		reader.onload = function (e) {
			$('#preview').attr('src', e.target.result);
			$("#removeLogo").val(0);
		};
	}
}
function removeImage(url) {
	$('#preview').attr('src', url);
	$("#removeLogo").val(1);
}