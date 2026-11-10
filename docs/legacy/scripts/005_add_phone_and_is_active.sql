-- телефон салона и флаг активности у салонов и услуг

ALTER TABLE businesses ADD COLUMN phone varchar(30);

-- активен ли салон
ALTER TABLE businesses ADD COLUMN is_active boolean NOT NULL DEFAULT true;

-- активна ли услуга
ALTER TABLE services ADD COLUMN is_active boolean NOT NULL DEFAULT true;