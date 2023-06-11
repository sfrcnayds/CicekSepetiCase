# Çiçeksepeti Case API Dokümantasyonu

Bu dokümantasyon, sepete ürün ekleme API'si hakkında bilgiler içermektedir.

## Projenin Çalıştırılması
    docker-compose up
komutu ile proje ayağa kalkmaktadır.

## API Erişimi
Proje docker'da çalıştıktan sonra link üzerinden swagger'a erişilebilir
- http://localhost:8080/swagger/index
## API Endpointi:

  - POST /ShoppingCart/AddProduct

## İstek Formatı:

API'ye gönderilen istek aşağıdaki JSON formatında olmalıdır:

{"userId": "GUID","shoppingCardAddProduct": {"productId": "GUID","quantity": "int"}

# Seed Datalar

## User Tablosu Dataları
| Id                                   	| Name     	| Surname 	| Username  	| EmailAddress        	|
|--------------------------------------	|----------	|---------	|-----------	|---------------------	|
| d9efde00-bb0e-4f30-8f4f-ed7ba7ac8599 	| Sefercan 	| Aydaş   	| sfrcnayds 	| sfrcnayds@gmail.com 	|
| 535fbd99-f400-4c1d-8a12-90bb82eabb53 	| Test     	| User    	| testuser  	| testuser@test.com   	|


## Product Tablosu Dataları

| Id                                   	| Name        	| Sku   	| Price 	| StockQuantity 	|
|--------------------------------------	|-------------	|-------	|-------	|---------------	|
| ca4e4352-85ea-4abb-aa98-bf843a57310a 	| Kırmızı Gül 	| KG727 	| 100   	| 5             	|
| 83dc5fd3-e294-40bb-94d8-d22087160383 	| Beyaz Gül   	| BG123 	| 200   	| 10            	|
