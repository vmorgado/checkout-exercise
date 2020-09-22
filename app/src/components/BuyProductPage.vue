<template>
  <v-app id="inspire">
    <v-container class="fill-height" fluid>
      <v-row align="center" justify="center">
        <v-col cols="12" sm="8" md="4">
          <v-card class="elevation-12">
            <v-img
              class="white--text align-end"
              height="200px"
              src="https://cdn.mos.cms.futurecdn.net/BT4GR5JKJJTudF6kUGEqg7-650-80.jpg"
            >
              <v-card-title>Checkout for Euler Identity</v-card-title>
            </v-img>

            <v-card-subtitle >Checkout for <b>Euler Identity</b></v-card-subtitle>
            
            <v-card-text class="text--primary">
              <p><b>Description:</b> become you, yourself, the one to be identified by Euler Identity!</p>

              <p><b>Price:</b> {{this.price}} {{this.currency}} </p>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <v-row align="center" justify="center">
        <v-col cols="12" sm="8" md="4">
          <PaymentForm
            :value="this.price"
            :currency="this.currency"
            :entity="this.entityId"
            :apiKey="this.apiKey"
          ></PaymentForm>
        </v-col>
      </v-row>
    </v-container>
  </v-app>
</template>
<script>
import PaymentForm from "./PaymentForm";

export default {
  name: "BuyProductPage",
  components: {
    PaymentForm,
  },
  data: () => ({
    price: 200,
    currency: "EURO",
    entityId: "market-app",
    apiKey: "C5BFF7F0-B4DF-475E-A331-F737424F013C",
  }),
};
</script>
