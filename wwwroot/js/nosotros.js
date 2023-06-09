function iniciarMap(){
    var coord = {lat:-12.1203744 ,lng: -77.0434558};
    var map = new google.maps.Map(document.getElementById('map'),{
      zoom: 10,
      center: coord
    });
    var marker = new google.maps.Marker({
      position: coord,
      map: map
    });
}