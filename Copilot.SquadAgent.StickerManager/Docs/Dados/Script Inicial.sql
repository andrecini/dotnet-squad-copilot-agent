  -- ============================================================
  -- Sticker Manager — Schema PostgreSQL
  -- ============================================================

  CREATE TABLE users (
      id              UUID            NOT NULL DEFAULT gen_random_uuid(),
      email           VARCHAR(256)    NOT NULL,
      name            VARCHAR(256)    NOT NULL,
      password_hash   VARCHAR(512)    NOT NULL,
      created_at      TIMESTAMP       NOT NULL,
      updated_at      TIMESTAMP       NULL,
      deleted_at      TIMESTAMP       NULL,

      CONSTRAINT pk_users PRIMARY KEY (id)
  );

  CREATE UNIQUE INDEX ix_users_email ON users (email);


  CREATE TABLE teams (
      id          UUID        NOT NULL DEFAULT gen_random_uuid(),
      name        VARCHAR(256) NOT NULL,
      code        CHAR(3)     NOT NULL,
      flag_url    VARCHAR(1024) NOT NULL,
      created_at  TIMESTAMP   NOT NULL,
      updated_at  TIMESTAMP   NULL,
      deleted_at  TIMESTAMP   NULL,

      CONSTRAINT pk_teams PRIMARY KEY (id)
  );


  CREATE TABLE stickers (
      id          UUID        NOT NULL DEFAULT gen_random_uuid(),
      number      INTEGER     NOT NULL,
      player_name VARCHAR(256) NOT NULL,
      rarity      VARCHAR(32) NOT NULL,
      team_id     UUID        NOT NULL,
      created_at  TIMESTAMP   NOT NULL,
      updated_at  TIMESTAMP   NULL,
      deleted_at  TIMESTAMP   NULL,

      CONSTRAINT pk_stickers          PRIMARY KEY (id),
      CONSTRAINT fk_stickers_team_id  FOREIGN KEY (team_id) REFERENCES teams (id)
  );

  CREATE INDEX ix_stickers_team_id ON stickers (team_id);


  CREATE TABLE user_collections (
      id                  UUID    NOT NULL DEFAULT gen_random_uuid(),
      user_id             UUID    NOT NULL,
      sticker_id          UUID    NOT NULL,
      quantity_owned      INTEGER NOT NULL,
      quantity_duplicate  INTEGER NOT NULL,
      created_at          TIMESTAMP NOT NULL,
      updated_at          TIMESTAMP NULL,
      deleted_at          TIMESTAMP NULL,

      CONSTRAINT pk_user_collections              PRIMARY KEY (id),
      CONSTRAINT fk_user_collections_user_id      FOREIGN KEY (user_id)    REFERENCES users (id),
      CONSTRAINT fk_user_collections_sticker_id   FOREIGN KEY (sticker_id) REFERENCES stickers (id)
  );

  CREATE UNIQUE INDEX ix_user_collections_user_id_sticker_id ON user_collections (user_id, sticker_id);
  CREATE INDEX        ix_user_collections_user_id             ON user_collections (user_id);
  CREATE INDEX        ix_user_collections_sticker_id          ON user_collections (sticker_id);


  CREATE TABLE trade_offers (
      id          UUID        NOT NULL DEFAULT gen_random_uuid(),
      user_id     UUID        NOT NULL,
      status      VARCHAR(32) NOT NULL,
      created_at  TIMESTAMP   NOT NULL,
      updated_at  TIMESTAMP   NULL,
      deleted_at  TIMESTAMP   NULL,

      CONSTRAINT pk_trade_offers          PRIMARY KEY (id),
      CONSTRAINT fk_trade_offers_user_id  FOREIGN KEY (user_id) REFERENCES users (id)
  );

  CREATE INDEX ix_trade_offers_user_id ON trade_offers (user_id);


  CREATE TABLE trade_offer_items (
      id              UUID        NOT NULL DEFAULT gen_random_uuid(),
      trade_offer_id  UUID        NOT NULL,
      sticker_id      UUID        NOT NULL,
      direction       VARCHAR(32) NOT NULL,
      created_at      TIMESTAMP   NOT NULL,
      updated_at      TIMESTAMP   NULL,
      deleted_at      TIMESTAMP   NULL,

      CONSTRAINT pk_trade_offer_items                 PRIMARY KEY (id),
      CONSTRAINT fk_trade_offer_items_trade_offer_id  FOREIGN KEY (trade_offer_id) REFERENCES trade_offers (id),
      CONSTRAINT fk_trade_offer_items_sticker_id      FOREIGN KEY (sticker_id)     REFERENCES stickers (id)
  );

  CREATE INDEX ix_trade_offer_items_trade_offer_id ON trade_offer_items (trade_offer_id);